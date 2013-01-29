/*
 * transaction.cc
 * 
 * Authors: Michael Banks and Dan Willoughby
 *
 *
 */

#ifndef TRANSACTION_HEADER_IKL
#define TRANSACTION_HEADER_IKL

#include "headquarters.h"
using namespace std;

class transaction
{
public:
  transaction();
  ~transaction();
  //@todo enum

private:
  int typeOfTransaction;   // Receive or request
  string upcCode;
  string theDate;
  int quantity;

};
#endif

transaction::transaction(){

}

transaction::~transaction(){

}
