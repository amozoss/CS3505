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
  transaction(string trans, string da_date, int shelf_life);
  ~transaction();

  enum transaction_type
  {
    request,
     receive
  };
  int get_type();
  string get_upc_code();
  string get_date();
  int get_quantity();
  int get_shelf_life();
  void dec_shelf_life();
private:
  int type_of_transaction;
  string upc_code;
  string the_date;
  int quantity;
  int shelf_life;

};
#endif
