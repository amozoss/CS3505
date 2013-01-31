/*
 * transaction.cc
 * 
 * Authors: Michael Banks and Dan Willoughby
 *
 *
 */

#include "transaction.h"
#include <vector>

transaction::transaction(string s)
{
  if(s[2] == 'c')
    type_of_transaction = receive;

  vector<string> words = s.split(' ');

  upc_code = words[2];

  cout << "Transaction type " << type_of_transaction << endl;

  cout << upc_code << endl;

  shelf_life = atoi(words[3].c_str());

  cout << shelf_life << " is the shelf life." << endl;




}

transaction::~transaction()
{

}



int transaction::get_type()
{
  return type_of_transaction;
}

int transaction::get_upc_code()
{

}

string transaction::get_date()
{

}

int transaction::get_quantity()
{

}
