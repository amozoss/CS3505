/*
 * Vector class definitions - This version is more complete than
 * the version from class.  It is largely uncommented.
 *
 * Peter
 */

#include "vector.h"
#include <iostream>
#include <sstream>


vector::vector (int x, int y)
{
  this->x = x;
  this->y = y;

  std::cout << "IN: vector::vector (int, int)" << std::endl;
}

vector::vector (const vector& v)
{
  std::cout << "IN: vector::vector (const vector&)" << std::endl;

  this->x = v.x;
  this->y = v.y;
}
  
vector::~vector ()
{
  std::cout << "IN: vector::~vector" << std::endl;
}

vector & vector::operator*= (double d)
{
  std::cout << "IN: vector::operator*= " << d << std::endl;

  x *= d;
  y *= d;
  return *this;
}

/* Overriding assignment where lhs is a vector, rhs is a vector */

vector& vector::operator= (const vector &rhs)
{
  std::cout << "IN: vector::operator= " << rhs << std::endl;

  this->x = rhs.x;
  this->y = rhs.y;
    
  return *this;
}

/* Overriding the operator to cast a vector to a string */

vector::operator std::string() const
{
  std::cout << "IN: vector::operator std::string" << std::endl;
  std::stringstream s;
  s << "(" << x << ", " << y << ")";
  return s.str();
}

std::ostream& operator<< (std::ostream &out, const vector &v)
{
  out << "(" << v.x << ", " << v.y << ")";
  return out;
}

/* Unary - */

const vector operator- (const vector &v)
{
  vector a(v);
  a.x = -a.x;
  a.y = -a.y;
  return a;
}

