/* Phillip Bradbury, Cody Foltz, Michael Banks, Dan Willoughby
 * CS3505
 * Final Project: Spreadsheet Server
 * server2.cpp
 *
 * Description: This server handles all incoming and outgoing communications.
 * Originally this was from the examples/ directory on boost.org, hence the
 * copyright info below.  There are two classes that are intertwined, server,
 * which essentially listens for incoming connections and creates asynchronous
 * threads for reading/writing from the associated sockets, and session, which
 * handles all communication between the server and the clients.
 */

//
// async_tcp_echo_server.cpp
// ~~~~~~~~~~~~~~~~~~~~~~~~~
//
// Copyright (c) 2003-2008 Christopher M. Kohlhoff (chris at kohlhoff dot com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
//

#include <cstdlib>
#include <iostream>
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include <queue>
#include <string>
#include "server.hpp"
#include <boost/tokenizer.hpp>
#include <boost/foreach.hpp>
#include <boost/lexical_cast.hpp>

#define CREATE "CREATE"
#define JOIN "JOIN"
#define CHANGE "CHANGE"
#define UNDO "UNDO"
#define SAVE "SAVE"
#define LEAVE "LEAVE"
#define ERROR_MESSAGE "ERROR\n"
#define NAME "Name:"
#define PASSWORD "Password:"
#define VERSION "Version:"
#define CELL "Cell:"
#define LENGTH "Length:"



using boost::asio::ip::tcp;
using namespace boost;

/* server constructor, directly copied from the async server example above
 * the only change was to add manager, which is a pointer to the spreadsheet
 * manager class implemented in spreadsheetManager.cc
 */
server::server(boost::asio::io_service& io_service, short port)
    : io_service_(io_service),
      acceptor_(io_service, tcp::endpoint(tcp::v4(), port))
  {
    manager = new spreadsheetManager();
    session* new_session = new session(io_service_, this);
    acceptor_.async_accept(new_session->socket(),
        boost::bind(&server::handle_accept, this, new_session,
          boost::asio::placeholders::error));
  }

/* handle_accept is unchanged from the async server example
 *
 */
void server::handle_accept(session* new_session,
      const boost::system::error_code& error)
  {
    if (!error)
    {
      new_session->start();
      new_session = new session(io_service_, this);
      acceptor_.async_accept(new_session->socket(),
          boost::bind(&server::handle_accept, this, new_session,
            boost::asio::placeholders::error));
    }
    else {
        delete new_session;
    }
}

/*
CREATE LF
Name:name LF
Password:password LF

JOIN LF
Name:name LF
Password:password LF

CHANGE LF
Name:name LF
Version:version LF
Cell:cell LF
Length:length LF
content LF

UNDO LF
Name:name LF
Version:version LF

SAVE LF
Name:name LF

LEAVE LF
Name:name LF
 */

/* log_cmd is called by a session that receives a command string terminated
 * with '\n'.  The command is split by '\n' into a vector containing all 
 * commands in that string.  Ideally, log_cmd will receive a complete command
 * from the client including all lines as listed above.  Otherwise, the server
 * will send back error messages.
 */
// @param: client   : for identifying the source of the message
// @param: msg      : actual command string
void server::log_cmd(session * client, std::string msg) {
    // for debugging purposes, checking commands received by the server from a
    // client
    std::cout << "Server received: " << msg << std::endl << std::endl;

    // vector containing the tokenized command string
    std::vector<std::string> messageVector = tokenize_String_By_LF( client, msg );
    // first message in the command string.
    std::string command = messageVector[0];

    // self-explanatory
    if (command.compare(CHANGE) == 0 ) {
        parse_Change_Command(client, messageVector);
    } else if (command.compare(CREATE) == 0 ) {
        parse_Create_Command(client, messageVector);
    } else if (command.compare(JOIN) == 0 ) {
        parse_Join_Command(client, messageVector);
    } else if (command.compare(UNDO) == 0 ) {
        parse_Undo_Command(client, messageVector);
    } else if (command.compare(SAVE) == 0 ) {
        parse_Save_Command(client, messageVector);
    } else if (command.compare(LEAVE) == 0 ) {
        parse_Leave_Command(client, messageVector);
    } else {
        send_Error(client);
    }

}

/* tokenize_String_By_LF accepts a client and a message
 * essentially just splits the string by a '\n' and returns a vector of all
 * strings
 */
std::vector<std::string> server::tokenize_String_By_LF(session* client, std::string message) {
    std::vector<std::string> messageVector;
    
    // using the boost libraries tokenizer and char_separator
    char_separator<char> sep("\n");
    tokenizer< char_separator<char> > tokens(message, sep);
    // iterate over the command string
    BOOST_FOREACH (const std::string& t, tokens) {
        messageVector.push_back(t);
    }
    
    return messageVector;
}

/** The tokenizing is from http://stackoverflow.com/questions/53849/how-do-i-tokenize-a-string-in-c by Ferruccio
 * Some tweeks by Cody.
 */

std::string server::strip_String_Header_With_Colon(std::string line){
    std::string result;
    char_separator<char> sep(":");
    tokenizer< char_separator<char> > tokens(line, sep);
    BOOST_FOREACH (const std::string& t, tokens) {
        result = t;
    }
    
  return result;
}

/* parse_Create_Command accepts a client and a vector of strings
 * this method will be called by log_cmd when the first message in the vector
 * is CREATE\n
 */
void server::parse_Create_Command(session* client, std::vector<std::string> messageVector) {
    /*
    CREATE LF
    Name:name LF
    Password:password LF
     */
    // There should only be 3 strings in the vector as shown above
    if(messageVector.size() == 3){
      // we can safely ignore the first message at index 0, since it is CREATE
        std::string name = messageVector[1];
        std::string password = messageVector[2];
	// ensure that there is an identifier Name: within the second string
        int index = name.find(NAME);
        if(index != 0){
	  // Name: isn't present, there was a problem in the message.
                client->send_msg(ERROR_MESSAGE);
                return;
        }
        
	// ensure there is an identifier Password: within the third string
        index = password.find(PASSWORD);
        if(index != 0){
	  // Password: isn't present, there was a problem
            client->send_msg(ERROR_MESSAGE);
            return;
        }
	// remove the Name: and Password: parts of the string so we are left
	// with relevant info.
        name = strip_String_Header_With_Colon(name);
        password = strip_String_Header_With_Colon(password);
        
	// call the spreadsheet create method
        manager->create_New_Spreadsheet_File(name, password, client);
        return;
    }
    // if there aren't 3 strings in the vector, then we need to send an error
    // and return.
    
    client->send_msg(ERROR_MESSAGE);
    return;
}
/* parse_Join_Command accepts a server and a vector of strings.  Used to handle
 * any join requests from client
 */
void server::parse_Join_Command(session* client, std::vector<std::string> messageVector) {
    /*
     JOIN LF
    Name:name LF
    Password:password LF
     */
    if(messageVector.size() == 3){
      // A proper join command will have 3 strings. Since the first is the JOIN
      // we can ignore it.
        std::string name = messageVector[1];
        std::string password = messageVector[2];
	// attempt to find Name: header in the string
        int index = name.find(NAME);
        if(index != 0){
	  // if the second string does not contain Name:, there was an error
	  // so send the error and return
                client->send_msg(ERROR_MESSAGE);
                return;
        }
        
	// attempt to find Password: header in the string
        index = password.find(PASSWORD);
        if(index != 0){
	  // if the third string doesn't contain Password:, there was an error
	  // so send the error and return
            client->send_msg(ERROR_MESSAGE);
            return;
        }
	// remove the Name: and Password: so that only relevant info is left
        name = strip_String_Header_With_Colon(name);
        password = strip_String_Header_With_Colon(password);
        
	// call the join method that handles everything and return
        manager->join_Spreadsheet(name, password, client);
        return;
    }
    // if there weren't 3 strings in the vector, there was a problem
    // send the error message and return
    client->send_msg(ERROR_MESSAGE);
    return;
}

/* parse_Change_Command accepts a session and vector of strings
 */
void server::parse_Change_Command(session* client, std::vector<std::string> messageVector) {
    /*
    CHANGE LF
    Name:name LF
    Version:version LF
    Cell:cell LF
    Length:length LF
    content LF
    */
    // There should be 6 elements in the vector
    if(messageVector.size() == 6){
        std::string name = messageVector[1];
        std::string version = messageVector[2];
        std::string cell = messageVector[3];
        std::string length = messageVector[4];
        std::string contents = messageVector[5];
        
        int index = name.find(NAME);
        //should start with NAME
        if(index != 0){
                client->send_msg(ERROR_MESSAGE);
                return;
        }
        
        //should start with VERSION
        index = version.find(VERSION);
        if(index != 0){
            client->send_msg(ERROR_MESSAGE);
            return;
        }
        
        //Should start with CELL
        index = cell.find(CELL);
        if(index != 0){
            client->send_msg(ERROR_MESSAGE);
            return;
        }
        
        //should start with LENGTH
        index = length.find(LENGTH);
        if(index != 0){
            client->send_msg(ERROR_MESSAGE);
            return;
        }
        
	using boost::lexical_cast;
	using boost::bad_lexical_cast;
	
	// remove the line identifier.
        name = strip_String_Header_With_Colon(name);
        version = strip_String_Header_With_Colon(version);
        cell = strip_String_Header_With_Colon(cell);
        length = strip_String_Header_With_Colon(length);
	int versionNumber;
	// attempt to cast 
	try {
	  versionNumber = boost::lexical_cast<int>(version);
	}
	// something went wrong, send an error message
	catch (bad_lexical_cast &){
	  versionNumber = -1;
	  client->send_msg(ERROR_MESSAGE);
	}
        
	// call the relevant change Spreadsheet method and return
        manager->change_Spreadsheet(name, client, versionNumber, cell, contents);
        return;
    }
    // if there weren't 6 strings in the vector, send an error message
    // to the client and return
    client->send_msg(ERROR_MESSAGE);
    return;
}


void server::parse_Undo_Command(session* client, std::vector<std::string> messageVector) {
    /*
     UNDO LF
    Name:name LF
    Version:version LF
     */
    if(messageVector.size() == 3){
        std::string name = messageVector[1];
        std::string version = messageVector[2];
        int index = name.find(NAME);
        if(index != 0){
                client->send_msg(ERROR_MESSAGE);
                return;
        }
        
        index = version.find(VERSION);
        if(index != 0){
            client->send_msg(ERROR_MESSAGE);
            return;
        }
        name = strip_String_Header_With_Colon(name);
        version = strip_String_Header_With_Colon(version);
        int versionNumber = boost::lexical_cast<int>(version);
        manager->undo_Spreadsheet_Change(name, versionNumber, client);
        return;
    }
    client->send_msg(ERROR_MESSAGE);
    return;
    
}

void server::parse_Save_Command(session* client, std::vector<std::string> messageVector) {
    /*
    SAVE LF
    Name:name LF
     */
    if(messageVector.size() == 2){
        std::string name = messageVector[1];
        int index = name.find(NAME);
        if(index != 0){
                client->send_msg(ERROR_MESSAGE);
                return;
        }
        
        name = strip_String_Header_With_Colon(name);
        
        manager->save_Spreadsheet(name, client);
        return;
    }
    client->send_msg(ERROR_MESSAGE);
    return;
}

void server::parse_Leave_Command(session* client, std::vector<std::string> messageVector) {
    /*
     LEAVE LF
    Name:name LF
     */
    if(messageVector.size() == 2){
        std::string name = messageVector[1];
        int index = name.find(NAME);
        if(index != 0){
                client->send_msg(ERROR_MESSAGE);
                return;
        }
        
        name = strip_String_Header_With_Colon(name);
        
        manager->disconnect_Client_From_Spreadsheet(name, client);
        return;
    }
    client->send_msg(ERROR_MESSAGE);
    return;
}
  void server::send_Error(session* client){
    std::string msg = "First message was not properly formatted\n";
      client->send_msg(msg);
  }


session::session(boost::asio::io_service& io_service, server* _s)
    : socket_(io_service), s(_s)
  {
    for (int i = 0; i < max_length; i++){
      data_[i] = 0;
    }
    isWriting = false;
  }

tcp::socket& session::socket()
  {
    return socket_;
  }

void session::start()
  {
    for (int i = 0; i < max_length; i++)
      data_[i] = 0;
    socket_.async_read_some(boost::asio::buffer(data_, max_length),
        boost::bind(&session::handle_read, this,
          boost::asio::placeholders::error,
          boost::asio::placeholders::bytes_transferred));
  }

session* session::get_Session(){
    return this;
}

void session::handle_read(const boost::system::error_code& error,
      size_t bytes_transferred)
  {
    if (!error)
    {
      if (!(socket_.is_open())){
	std::cout << "socket is not open" << std::endl;
	for (int i = 0; i < 1024; i++)
	  data_[i] = 0;
	while (!so_far.empty())
	  so_far.pop();
      }

      /*
      boost::asio::async_write(socket_,
          boost::asio::buffer(data_, bytes_transferred),
          boost::bind(&session::handle_write, this,
            boost::asio::placeholders::error));
      */

      pthread_mutex_lock(&readLock);
      std::string msg(data_);
      so_far.push(msg);
      pthread_mutex_unlock(&readLock);
      size_t found = msg.find_first_of('\n');
      std::string command = "";
      if (found != std::string::npos){
	pthread_mutex_lock(&readLock);
	while (!so_far.empty()){
	  command += so_far.front();
	  so_far.pop();
	}
      pthread_mutex_unlock(&readLock);
      }

      if (!(command == "")){
	s->log_cmd(this, command);
      }
      // This clears out the buffer
      for (int i = 0; i < max_length; i++)
	data_[i] = 0;

      // since this is the only place where we call the async_read_some
      // method, data_ is guaranteed to be cleared in the previous for
      // loop.
      socket_.async_read_some(boost::asio::buffer(data_, max_length),
          boost::bind(&session::handle_read, this,
            boost::asio::placeholders::error,
            boost::asio::placeholders::bytes_transferred));
    }
    else
    {
      if (!(socket_.is_open())){
	std::cout << "socket is not open" << std::endl;
	for (int i = 0; i < 1024; i++)
	  data_[i] = 0;
	while (!so_far.empty())
	  so_far.pop();
      }
      s->manager->disconnect_Client_From_All(this);
    }
  }

void session::send_msg(std::string msg){
  if (!(socket_.is_open())){
    std::cout << "socket is not open" << std::endl;
    for (int i = 0; i < 1024; i++)
      data_[i] = 0;
    while (!handle_queue.empty()){
      handle_queue.pop();
    }
    while (!write_queue.empty()){
      write_queue.pop();
    }
  }

  size_t msg_length = 0;
  msg_length = msg.length();

  char * new_msg = new char [msg_length + 1];
  std::strcpy (new_msg, msg.c_str());
  q_msg m = { new_msg, msg_length };
  isWriting = false;

  if (!isWriting){
    pthread_mutex_lock(&writeLock);
    isWriting = true;
    write_queue.push(m);
    bool isEmpty = false;
    pthread_mutex_unlock(&writeLock);
    while (!isEmpty){
      pthread_mutex_lock(&writeLock);
      m = write_queue.front();
      write_queue.pop();
      isEmpty = write_queue.empty();
      pthread_mutex_unlock(&writeLock);

      pthread_mutex_lock(&handleLock);
      q_msg handle_msg = { m.msg, m.length };
      handle_queue.push(handle_msg);
      pthread_mutex_unlock(&handleLock);
      
      std::cout << m.msg << std::endl;

      boost::asio::async_write(socket_,
	 boost::asio::buffer(m.msg, m.length),
         boost::bind(&session::handle_write, this,
		      boost::asio::placeholders::error));
    }
    isWriting = false;
  }
  else {
    pthread_mutex_lock(&writeLock);
    write_queue.push(m);
    pthread_mutex_unlock(&writeLock);
  }
}


void session::handle_write(const boost::system::error_code& error)
  {
    if (!error)
    {
      // delete the buffer.
      pthread_mutex_lock(&handleLock);
      q_msg m = handle_queue.front();
      handle_queue.pop();
      pthread_mutex_unlock(&handleLock);

      delete m.msg;
    }
    else {
      if (!(socket_.is_open())){
	std::cout << "socket is not open" << std::endl;
	for (int i = 0; i < 1024; i++)
	  data_[i] = 0;
	while (!handle_queue.empty()){
	  handle_queue.pop();
	}
	while (!write_queue.empty()){
	  write_queue.pop();
	}
      }
      // send info to server so it can dispose of the session properly
      s->manager->disconnect_Client_From_All(this);
      //delete this;
    }
  }

