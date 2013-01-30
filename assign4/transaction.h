/*
 * transaction.h
 *
 * Manages receive and request transactions
 * 
 * Authors: Michael Banks and Dan Willoughby
 *
 *
 */
#ifndef TRANSACTION_HEADER_IKL
#define TRANSACTION_HEADER_IKL
#include <string>
using namespace std;

class transaction
{
public:
  transaction(string);
  ~transaction();
  enum transaction_type
  {
    request,
     receive
  }


private:
  int typeOfTransaction;   // Receive or request
  string upcCode;
  string theDate;
  int quantity;

};
#endif
