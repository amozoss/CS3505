#ifndef SERVER_H
#define SERVER_H

#include <cstdlib>
#include <iostream>
#include <boost/bind.hpp>
#include <boost/asio.hpp>
#include <queue>
#include <vector>
#include <string>
#include "spreadsheetManager.h"

using boost::asio::ip::tcp;

typedef struct queue_message{
  char* msg;
  int length;
} q_msg;

class server;
class spreadsheetManager;

class session
{
  friend class server;
public:
  session(boost::asio::io_service& io_service, server* _s);
  tcp::socket& socket();
  void start();
  void handle_read(const boost::system::error_code& error,
		   size_t bytes_transferred);
  void send_msg(std::string msg);
  void handle_write(const boost::system::error_code& error);
  session* get_Session();
private:
  tcp::socket socket_;
  enum { max_length = 1024 };
  char data_[max_length];
  std::queue<std::string> so_far;
  char * out_buf;
  server* s;

  bool isWriting;
  //std::queue<q_msg> read_queue;
  std::queue<q_msg> write_queue;
  std::queue<q_msg> handle_queue;
  pthread_mutex_t readLock, writeLock, handleLock;
};

typedef struct client_message{
  session * client;
  std::string msg;
} client_msg;

class server
{
  friend class session;

public:
  server(boost::asio::io_service& io_service, short port);
  void handle_accept(session* new_session,
		     const boost::system::error_code& error);
  void log_cmd(session * client, std::string);

private:
  spreadsheetManager* manager;

  boost::asio::io_service& io_service_;
  tcp::acceptor acceptor_;
  pthread_mutex_t clientCommandLock;
  std::queue<client_msg> client_command_queue;
  
  //helper methods
  std::vector<std::string> tokenize_String_By_LF(session* client, std::string message);
  std::string strip_String_Header_With_Colon(std::string line);
  void parse_Create_Command( session* client, std::vector<std::string> message_Vector);
  void parse_Join_Command( session* client, std::vector<std::string> message_Vector);
  void parse_Change_Command( session* client, std::vector<std::string> message_Vector);
  void parse_Undo_Command( session* client, std::vector<std::string> message_Vector);
  void parse_Save_Command( session* client, std::vector<std::string> message_Vector);
  void parse_Leave_Command( session* client, std::vector<std::string> message_Vector);
  void send_Error(session* client);
  
  
  
};

#endif
