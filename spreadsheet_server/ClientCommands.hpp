/* 
 * File:   ClientCommands.hpp
 * Author: codyfoltz
 *
 * Created on April 19, 2013, 4:36 PM
 */

#ifndef CLIENTCOMMANDS_HPP
#define	CLIENTCOMMANDS_HPP

#include <boost/lexical_cast.hpp>

namespace ClientCommands{
    
    std::string create_Join_FAIL_Command(std::string spreadsheetName, std::string message);
    std::string create_Join_OK_Command(std::string spreadsheetName, int version, std::string xml);
    std::string create_Create_OK_Command(std::string spreadsheetName, std::string password);
    std::string create_Create_FAIL_Command(std::string spreadsheetName, std::string message);
    std::string create_Change_OK_Command(std::string spreadsheetName, int version);
    std::string create_Change_FAIL_Command(std::string spreadsheetName, std::string message);
    std::string create_Change_WAIT_Command(std::string spreadsheetName, int version);
    std::string create_Undo_OK_Command(std::string spreadsheetName, int version, std::string cellName, std::string cellContents );
    std::string create_Undo_END_Command(std::string spreadsheetName, int version);  
    std::string create_Undo_WAIT_Command(std::string spreadsheetName, int version);   
    std::string create_Undo_FAIL_Command(std::string spreadsheetName, std::string message);
    std::string create_Update_Command(std::string spreadsheetName, int version, std::string cellName, std::string cellContents);
    std::string create_Save_OK_Command(std::string spreadsheetName);
    std::string create_Save_FAIL_Command(std::string spreadsheetName, std::string message);
    
}

#endif	/* CLIENTCOMMANDS_HPP */


//JOIN SP OK LF
//Name:name LF
//Version:version LF
//Length:length LF
//xml LF
