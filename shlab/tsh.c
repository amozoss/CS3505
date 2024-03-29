/* 
 * tsh - A tiny shell program with job control
 * 
 *  Dan Willoughby dwilloug
 *  Michael Banks mbanks
 */
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>

#include <string.h>
#include <ctype.h>
#include <signal.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <errno.h>

/* Misc manifest constants */
#define MAXLINE    1024   /* max line size */
#define MAXARGS     128   /* max args on a command line */
#define MAXJOBS      16   /* max jobs at any point in time */
#define MAXJID    1<<16   /* max job ID */

/* Job states */
#define UNDEF 0 /* undefined */
#define FG 1    /* running in foreground */
#define BG 2    /* running in background */
#define ST 3    /* stopped */

/* 
// * Jobs states: FG (foreground), BG (background), ST (stopped)
// * Job state transitions and enabling actions:
// *     FG -> ST  : ctrl-z
// *     ST -> FG  : fg command
// *     ST -> BG  : bg command
// *     BG -> FG  : fg command
// * At most 1 job can be in the FG state.
// */

/* Global variables */
extern char **environ;      /* defined in libc */
char prompt[] = "tsh> ";    /* command line prompt (DO NOT CHANGE) */
int verbose = 0;            /* if true, print additional output */
int debug = 0;
int nextjid = 1;            /* next job ID to allocate */
char sbuf[MAXLINE];         /* for composing sprintf messages */

struct job_t {              /* The job struct */
  pid_t pid;              /* job PID */
  int jid;                /* job ID [1, 2, ...] */
  int state;              /* UNDEF, BG, FG, or ST */
  char cmdline[MAXLINE];  /* command line */
};
struct job_t jobs[MAXJOBS]; /* The job list */
/* End global variables */


/* Function prototypes */

/* Here are the functions that you will implement */

void eval(char *cmdline);
int builtin_cmd(char **argv);
void do_bgfg(char **argv);
void waitfg(pid_t pid);
void sigchld_handler(int sig);
void sigtstp_handler(int sig);
void sigint_handler(int sig);


/* ***************************************************** */
/* Here are helper routines that we've provided for you */
int parseline(const char *cmdline, char **argv); 
void sigquit_handler(int sig);

void clearjob(struct job_t *job);
void initjobs(struct job_t *jobs);
int maxjid(struct job_t *jobs); 
int addjob(struct job_t *jobs, pid_t pid, int state, char *cmdline);
int deletejob(struct job_t *jobs, pid_t pid); 
pid_t fgpid(struct job_t *jobs);
struct job_t *getjobpid(struct job_t *jobs, pid_t pid);
struct job_t *getjobjid(struct job_t *jobs, int jid); 
int pid2jid(pid_t pid); 
void listjobs(struct job_t *jobs);

void usage(void);
void unix_error(char *msg);
void app_error(char *msg);
typedef void handler_t(int); 
handler_t *Signal(int signum, handler_t *handler);

/* ****************************************************** */
/* Custom helpers */
void changejobstate(pid_t pid, int state);

/*
 * main - The shell's main routine 
 */
int main(int argc, char **argv) 
{
  char c;
  char cmdline[MAXLINE];
  int emit_prompt = 1; /* emit prompt (default) */

  /* Redirect stderr to stdout (so that driver will get all output
   * on the pipe connected to stdout) */
  dup2(1, 2);

  /* Parse the command line */
  while ((c = getopt(argc, argv, "hvp")) != EOF) {
    switch (c) {
      case 'h':             /* print help message */
        usage();
        break;
      case 'v':             /* emit additional diagnostic info */
        verbose = 1;
        break;
      case 'p':             /* don't print a prompt */
        emit_prompt = 0;  /* handy for automatic testing */
        break;
      default:
        usage();
    }
  }

  /* Install the signal handlers */

  /* These are the ones you will need to implement */
  Signal(SIGINT,  sigint_handler);   /* ctrl-c */
  Signal(SIGTSTP, sigtstp_handler);  /* ctrl-z */
  Signal(SIGCHLD, sigchld_handler);  /* Terminated or stopped child */

  /* This one provides a clean way to kill the shell */
  Signal(SIGQUIT, sigquit_handler); 

  /* Initialize the job list */
  initjobs(jobs);

  /* Execute the shell's read/eval loop */
  while (1) {

    /* Read command line */
    if (emit_prompt) {
      printf("%s", prompt);
      fflush(stdout);
    }
    if ((fgets(cmdline, MAXLINE, stdin) == NULL) && ferror(stdin))
      app_error("fgets error");
    if (feof(stdin)) { /* End of file (ctrl-d) */
      fflush(stdout);
      exit(0);
    }

    /* Evaluate the command line */
    eval(cmdline);
    fflush(stdout);
    fflush(stdout);
  } 

  exit(0); /* control never reaches here */
}

/* Print stars has no \n, it creates a bunch of stars to split up debugging*/
void print_stars()
{
  fflush(stdout);
  printf("- - - - - - - - - - - - - - - - - - - - - - -");
}

/* forkwrapper from csapp.c*/
pid_t Fork(void) 
{
  pid_t pid;

  if ((pid = fork()) < 0)
    unix_error("Fork error");
  return pid;
}

/* 
 * eval - Evaluate the command line that the user has just typed in
 * 
 * If the user has requested a built-in command (quit, jobs, bg or fg)
 * then execute it immediately. Otherwise, fork a child process and
 * run the job in the context of the child. If the job is running in
 * the foreground, wait for it to terminate and then return.  Note:
 * each child process must have a unique process group ID so that our
 * background children don't receive SIGINT (SIGTSTP) from the kernel
 * when we type ctrl-c (ctrl-z) at the keyboard.  
 */
/* Main routine that parses and interprets the command line. 70 lines */ 
void eval(char *cmdline) 
{
  int isBG;
  pid_t pid;

  if (debug) {
    print_stars();
    printf("%s: Line %d --> %s\n", __func__, __LINE__, cmdline);
  }

  char *argv[MAXARGS];

  int state = UNDEF;
  isBG = parseline(cmdline, argv);
  state = (!isBG) ? FG : BG;

  sigset_t mask;
  if (sigemptyset(&mask) < 0)
    unix_error("Sigemptyset error");
  if (sigaddset(&mask, SIGCHLD) < 0)
    unix_error("Sigaddset error");

  if(!builtin_cmd(argv)) { 

    if (sigprocmask(SIG_BLOCK, &mask, NULL) < 0) // Block SIGCHLD 
      unix_error("Sigprocmask error");

    if((pid = Fork()) == 0) { // Fork is a helper method from csapp.c included above

      // setpgid is a workaround that puts the child in a new process group, ensures only one process in foreground group
      setpgid(0, 0);

      if (sigprocmask(SIG_UNBLOCK, &mask, NULL) < 0) // Unblock SIGCHLD 
        unix_error("Sigprocmask error");

      // if execve returns < 0 the command is not built in
      if((execve(argv[0], argv, environ)) < 0) {
        printf("%s: Command not found\n", argv[0]);
        fflush(stdout);
        exit(0);
      } 
    }

    addjob(jobs, pid, state, cmdline);
    if (sigprocmask(SIG_UNBLOCK, &mask, NULL) < 0) // Unblock SIGCHLD 
      unix_error("Sigprocmask error");

    // the state will be either bg or fg depending on the isBG bool above
    if(!isBG) {
      waitfg(pid);	
    }
    else {
      int jid = pid2jid(pid); // this should be after addjob
      fflush(stdout);
      printf("[%d] (%d) %s",jid, pid, cmdline);
    }

  }
  return;
}

/* 
 * parseline - Parse the command line and build the argv array.
 * 
 * Characters enclosed in single quotes are treated as a single
 * argument.  Return true if the user has requested a BG job, false if
 * the user has requested a FG job.  
 */
int parseline(const char *cmdline, char **argv) 
{
  static char array[MAXLINE]; /* holds local copy of command line */
  char *buf = array;          /* ptr that traverses command line */
  char *delim;                /* points to first space delimiter */
  int argc;                   /* number of args */
  int bg;                     /* background job? */

  strcpy(buf, cmdline);
  buf[strlen(buf)-1] = ' ';  /* replace trailing '\n' with space */
  while (*buf && (*buf == ' ')) /* ignore leading spaces */
    buf++;

  /* Build the argv list */
  argc = 0;
  if (*buf == '\'') {
    buf++;
    delim = strchr(buf, '\'');
  }
  else {
    delim = strchr(buf, ' ');
  }

  while (delim) {
    argv[argc++] = buf;
    *delim = '\0';
    buf = delim + 1;
    while (*buf && (*buf == ' ')) /* ignore spaces */
      buf++;

    if (*buf == '\'') {
      buf++;
      delim = strchr(buf, '\'');
    }
    else {
      delim = strchr(buf, ' ');
    }
  }
  argv[argc] = NULL;

  if (argc == 0)  /* ignore blank line */
    return 1;

  /* should the job run in the background? */
  if ((bg = (*argv[argc-1] == '&')) != 0) {
    argv[--argc] = NULL;
  }
  return bg;
}

/* 
 * builtin_cmd - If the user has typed a built-in command then execute
 *    it immediately.  
 */
/* Recognizes and interprets the built-in commands: quit, fg bg and jobs. 25 lines */
int builtin_cmd(char **argv) 
{
  int returnvar = 0;

  if(debug) {
    print_stars();
    printf("%s: %d\n", __func__, __LINE__);
  }

  if(!strcmp("quit", argv[0]))  	// strcmp returns 0 if they are equal
    exit(0);
  else if(!strcmp("bg", argv[0]) || !strcmp("fg", argv[0])) {
    do_bgfg(argv);
    returnvar = 1;
  }
  else if(!strcmp("jobs", argv[0])) {
    listjobs(jobs);
    returnvar = 1;
  }
  return returnvar;     /* Return whether or not the command was built in */
}

/* Returns the id 1-16 if it is legit, or 0 if the input isn't ok */
int get_id(char *argv)
{
  return (argv[0] == '%') ? atoi(argv+1) : atoi(argv);
}

/* 
 * do_bgfg - Execute the builtin bg and fg commands
 */
/* Implements the bg and fg built-in commands. 50 lines */
void do_bgfg(char **argv) 
{
  if(argv[1] == NULL) {  // handle no argument case
    printf("%s command requires PID or %%jobid argument\n", argv[0]);  
    return;  
  }   

  if(debug) {
    print_stars();
    printf("%s: %s %c\n", __func__, argv[0], argv[1][1]);
    fflush(stdout);
  }

  struct job_t *ajob;
  int id;
  // Get the jid or pid of the command line.
  id = get_id(argv[1]);
  
  if (id <= 0) { // Return if not valid
    printf("%s: argument must be a PID or %%jobid\n", argv[0]);
    return;
  }
  else if (argv[1][0] == '%')  // id was a jid
    ajob = getjobjid(jobs, id);
  else                        // id was a pid
    ajob = getjobpid(jobs, id);
  
  int state = (!strcmp(argv[0], "bg")) ? BG : FG;

  if (ajob != NULL) {       // If ajob != NULL, the job is in the job list.

    int jid = (* ajob).jid; 
    pid_t pid = (* ajob).pid;

    changejobstate(pid, state);
    if (kill(-(pid), SIGCONT) < 0) // send signal to process
      unix_error("kill error");

    if (state == BG) {                // If it was the 'bg' command, print crap about it.
      printf("[%d] (%d) %s",jid, pid, (* ajob).cmdline);
    }
    else if (state == FG) {           // If it was the 'fg' command, wait for it to die.
      waitfg(pid); 
    }
  }
  else {                     // if ajob == NULL, the job is not in the job list. 
    if(argv[1][0] == '%')
      printf("%%%d: No such job\n", id);
    else
      printf("(%d): No such process\n", id);
  }

  if(debug) {
    print_stars();
    printf("%s: %d, end of function\n", __func__, __LINE__);
    fflush(stdout);
  }

  return;
}

/* 
 * waitfg - Block until process pid is no longer the foreground process
 */
/* Waits for a foreground job to complete. 20 lines */ 
void waitfg(pid_t pid)
{
  while(pid == fgpid(jobs))
    sleep(0);
}


/* Job states */
//#define UNDEF 0 /* undefined */
//#define FG 1    /* running in foreground */
//#define BG 2    /* running in background */
//#define ST 3    /* stopped */

/* 
// * Jobs states: FG (foreground), BG (background), ST (stopped)
// * Job state transitions and enabling actions:
// *     FG -> ST  : ctrl-z
// *     ST -> FG  : fg command
// *     ST -> BG  : bg command
// *     BG -> FG  : fg command
// * At most 1 job can be in the FG state.
// */
/*****************
 * Signal handlers
 *****************/

/* 
 * sigchld_handler - The kernel sends a SIGCHLD to the shell whenever
 *     a child job terminates (becomes a zombie), or stops because it
 *     received a SIGSTOP or SIGTSTP signal. The handler reaps all
 *     available zombie children, but doesn't wait for any other
 *     currently running children to terminate.  
 */
/* Catches SIGCHILD signals. 80 lines */ 
void sigchld_handler(int sig) 
{
  if(debug) {
    print_stars();
    printf("%s: %d\n", __func__, __LINE__);
  }

  pid_t pid = 1;
  int status;
  int jid = 0; 
  
  while ((pid = waitpid(-1, &status, WNOHANG | WUNTRACED)) > 0) { // Reap a zombie child 

    jid = pid2jid(pid);

    if(WIFEXITED(status)) // child exited normally
      deletejob(jobs,pid);
    else if(WIFSIGNALED(status)) { //  the child exited because a signal was not caught. 
      if (WTERMSIG(status) == 2)   // Checks if termination was caused by SIGINT 
        printf("Job [%d] (%d) terminated by signal %d\n", jid, pid, 2);
      
      deletejob(jobs,pid);
    }
    else if(WIFSTOPPED(status)) {    // Determines if the child that caused the return is currently stopped. 
      changejobstate(pid, ST);
      printf("Job [%d] (%d) stopped by signal %d\n", jid, pid, WSTOPSIG(status));
    }
    fflush(stdout);
  }


  if (errno != ECHILD)
    unix_error("waitpid error");

  return;
}

/* 
 * sigint_handler - The kernel sends a SIGINT to the shell whenever the
 *    user types ctrl-c at the keyboard.  Catch it and send it along
 *    to the foreground job.  
 */
/* Catches SIGINT (ctrl-c) signals. 15 lines */
void sigint_handler(int sig) 
{
  if(debug) {
    print_stars();
    printf("%s: %d\n",__func__, __LINE__ );
    fflush(stdout);
  }

  pid_t pid = fgpid(jobs);

  if(sig == SIGINT && pid != 0) {  // Make sure there was a foreground job, and also make
                              // sure the signal is SIGINT.
    fflush(stdout);
    if (kill(-(pid), SIGINT) < 0) // send the kill signal
      unix_error("kill error");
  }

  if(debug) { 
    print_stars();
    printf("sig= %d pid =%d getpid() = %d\n",sig,pid,getpid());
    fflush(stdout);
  }



  return;
}

/*
 * sigtstp_handler - The kernel sends a SIGTSTP to the shell whenever
 *     the user types ctrl-z at the keyboard. Catch it and suspend the
 *     foreground job by sending it a SIGTSTP.  
 */
/* Catches SIGTSTP (ctrl-z) signals. 15 lines */
void sigtstp_handler(int sig) 
{
  if(debug) {
    print_stars();
    printf("%s: %d\n", __func__, __LINE__);
    fflush(stdout);
  }

  pid_t pid = fgpid(jobs);      // Retrieve the pid of the job running in the foreground.

  if(pid  == 0)                 // If the pid == 0, that means there isn't a foreground job.
    return;

  if (kill(-(pid), SIGTSTP) < 0) // Send the SIGTSTP signal to the process
    unix_error("kill error");

  if(debug) {
    print_stars();
    printf("sig= %d pid =%d getpid() = %d\n",sig,pid,getpid());
    fflush(stdout);
  }
  
  return;
}

/*********************
 * End signal handlers
 *********************/

/***********************************************
 * Helper routines that manipulate the job list
 **********************************************/
void changejobstate(pid_t pid, int state) {
  struct job_t *ajob = getjobpid(jobs, pid);

  if(ajob != NULL) 
    (* ajob).state = state;

  if(debug) {
    print_stars();
    printf("%s [%d] (%d) %s\n",__func__,  ajob[0].jid, ajob[0].pid, ajob[0].cmdline);
  }
}

/* clearjob - Clear the entries in a job struct */
void clearjob(struct job_t *job) {
  job->pid = 0;
  job->jid = 0;
  job->state = UNDEF;
  job->cmdline[0] = '\0';
}

/* initjobs - Initialize the job list */
void initjobs(struct job_t *jobs) {
  int i;

  for (i = 0; i < MAXJOBS; i++)
    clearjob(&jobs[i]);
}

/* maxjid - Returns largest allocated job ID */
int maxjid(struct job_t *jobs) 
{
  int i, max=0;

  for (i = 0; i < MAXJOBS; i++)
    if (jobs[i].jid > max)
      max = jobs[i].jid;
  return max;
}

/* addjob - Add a job to the job list */
int addjob(struct job_t *jobs, pid_t pid, int state, char *cmdline) 
{
  int i;

  if (pid < 1)
    return 0;

  for (i = 0; i < MAXJOBS; i++) {
    if (jobs[i].pid == 0) {
      jobs[i].pid = pid;
      jobs[i].state = state;
      jobs[i].jid = nextjid++;
      if (nextjid > MAXJOBS)
        nextjid = 1;
      strcpy(jobs[i].cmdline, cmdline);
      if(verbose){
        print_stars();
        printf("Added job [%d] %d %s\n", jobs[i].jid, jobs[i].pid, jobs[i].cmdline);
      }
      return 1;
    }	
  }
  fflush(stdout);
  printf("Tried to create too many jobs\n");
  return 0;
}

/* deletejob - Delete a job whose PID=pid from the job list */
int deletejob(struct job_t *jobs, pid_t pid) 
{
  int i;
  if(verbose) {
    print_stars();
    printf("delete job[%d]\n", pid);
    fflush(stdout);
  }

  if (pid < 1)
    return 0;

  for (i = 0; i < MAXJOBS; i++) {
    if (jobs[i].pid == pid) {
      clearjob(&jobs[i]);
      nextjid = maxjid(jobs)+1;
      return 1;
    }
  }
  return 0;
}

/* fgpid - Return PID of current foreground job, 0 if no such job */
pid_t fgpid(struct job_t *jobs) {
  int i;

  for (i = 0; i < MAXJOBS; i++)
    if (jobs[i].state == FG)
      return jobs[i].pid;
  return 0;
}

/* getjobpid  - Find a job (by PID) on the job list */
struct job_t *getjobpid(struct job_t *jobs, pid_t pid) {
  int i;

  if (pid < 1)
    return NULL;
  for (i = 0; i < MAXJOBS; i++)
    if (jobs[i].pid == pid)
      return &jobs[i];
  return NULL;
}

/* getjobjid  - Find a job (by JID) on the job list */
struct job_t *getjobjid(struct job_t *jobs, int jid) 
{
  int i;

  if (jid < 1)
    return NULL;
  for (i = 0; i < MAXJOBS; i++)
    if (jobs[i].jid == jid)
      return &jobs[i];
  return NULL;
}

/* pid2jid - Map process ID to job ID */
int pid2jid(pid_t pid) 
{
  int i;

  if (pid < 1)
    return 0;
  for (i = 0; i < MAXJOBS; i++)
    if (jobs[i].pid == pid) {
      return jobs[i].jid;
    }
  return 0;
}

/* listjobs - Print the job list */
void listjobs(struct job_t *jobs) 
{
  int i;

  for (i = 0; i < MAXJOBS; i++) 
  {
    fflush(stdout);
    if (jobs[i].pid != 0) {
      printf("[%d] (%d) ", jobs[i].jid, jobs[i].pid);
      switch (jobs[i].state) {
        case BG: 
          printf("Running ");
          break;
        case FG: 
          printf("Foreground ");
          break;
        case ST: 
          printf("Stopped ");
          break;
        default:
          printf("listjobs: Internal error: job[%d].state=%d ", 
              i, jobs[i].state);
      }
      printf("%s", jobs[i].cmdline);
      fflush(stdout);
    }
  }
}
/******************************
 * end job list helper routines
 ******************************/


/***********************
 * Other helper routines
 ***********************/

/*
 * usage - print a help message
 */
void usage(void) 
{
  fflush(stdout);
  printf("Usage: shell [-hvp]\n");
  printf("   -h   print this message\n");
  printf("   -v   print additional diagnostic information\n");
  printf("   -p   do not emit a command prompt\n");
  fflush(stdout);
  exit(1);
}

/*
 * unix_error - unix-style error routine
 */
void unix_error(char *msg)
{
  fflush(stdout);
  fprintf(stdout, "%s: %s\n", msg, strerror(errno));
  exit(1);
}

/*
 * app_error - application-style error routine
 */
void app_error(char *msg)
{
  fflush(stdout);
  fprintf(stdout, "%s\n", msg);
  exit(1);
}

/*
 * Signal - wrapper for the sigaction function
 */
handler_t *Signal(int signum, handler_t *handler) 
{
  struct sigaction action, old_action;

  action.sa_handler = handler;  
  sigemptyset(&action.sa_mask); /* block sigs of type being handled */
  action.sa_flags = SA_RESTART; /* restart syscalls if possible */

  if (sigaction(signum, &action, &old_action) < 0)
    unix_error("Signal error");
  return (old_action.sa_handler);
}

/*
 * sigquit_handler - The driver program can gracefully terminate the
 *    child shell by sending it a SIGQUIT signal.
 */
void sigquit_handler(int sig) 
{
  fflush(stdout);
  printf("Terminating after receipt of SIGQUIT signal\n");
  exit(1);
}



