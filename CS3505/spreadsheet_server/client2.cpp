#include <iostream>
#include <boost/array.hpp>
#include <boost/asio.hpp>

namespace
{
    const char* HELLO_PORT_STR = "1984";
}

int main(int argc, char** argv){
  try
  {

    char* host = argv[1];
  boost::asio::io_service io_service; // 1.

  boost::asio::ip::tcp::resolver resolver(io_service); // 2.
  boost::asio::ip::tcp::resolver::query query(host, HELLO_PORT_STR); // 3.
  boost::asio::ip::tcp::resolver::iterator endpoint_iterator = resolver.resolve(query);
  boost::asio::ip::tcp::resolver::iterator end; // 4.

  boost::asio::ip::tcp::socket socket(io_service);
  boost::system::error_code error = boost::asio::error::host_not_found;
 while(error && endpoint_iterator != end) // 5.
  {
    socket.close();
    socket.connect(*endpoint_iterator++, error);
  }
 if(error) // 6.
    throw boost::system::system_error(error);

 for(;;) // 7.
  {
    boost::array<char, 4> buf; // 8.
    size_t len = socket.read_some(boost::asio::buffer(buf), error); // 9.

    if(error == boost::asio::error::eof) // 10.
        break; // Connection closed cleanly by peer.
    else if(error)
        throw boost::system::system_error(error);

    //std::cout << "[!]"; // 11.
    std::cout.write(buf.data(), len); // 12.
  }
  }
 catch(std::exception& e)
  {
  std::cerr << e.what() << std::endl;
  }
}
