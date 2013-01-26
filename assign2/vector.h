/*
 * Represents a coordinate.  Note -- this is my version
 * that I prepared prior to lecture.  It has a few extra functions in it.
 *
 * Remember, class functions declared here need to be defined later.
 * Class variables are declared here, and automatically defined whenever someone
 * makes a vector object.
 *
 * Finally, I originally put this in a namespace, but since I didn't get
 * to that in lecture, I've removed it.
 */

// These next few lines control how the compiler receives the text
// from this file.

// Is this label not defined?
#ifndef VECTOR_H

// If not, define it and compile the rest of it.
#define VECTOR_H

#include <ostream>
#include <string>

class vector
{
public:
    vector (int x, int y);
    vector (const vector& v);
    ~vector ();
    
    // int getX ();  // I forgot to add these accessor functions - duh!
    // int getY ();
    
    vector & operator=(const vector &rhs);
    vector & operator*= (double d);
    operator std::string() const;
    
    friend std::ostream& operator<< (std::ostream &out, const vector &v);
    friend const vector operator- (const vector &v);
    
    
private:
    int x;  // Declaring these here will make them part of any vector object.
    int y;
};

// Marks the end of the compiler flags.
#endif
