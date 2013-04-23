/* 
 * File:   ClientCommands.cc
 * Author: codyfoltz
 * 
 * Created on April 19, 2013, 4:36 PM
 */

#include "ClientCommands.hpp"

namespace ClientCommands {

    std::string create_Join_FAIL_Command(std::string spreadsheetName, std::string message) {

        //JOIN SP FAIL LF
        //Name:name LF
        //message LF
        std::string response = "JOIN FAIL\nName:";
        response += spreadsheetName;
        response += "\n";
        response += message;
        response += "\n";
        
        return response;
    }

    std::string create_Join_OK_Command(std::string spreadsheetName, int version, std::string xml) {
        int xmlLength = xml.length();
        std::string length = boost::lexical_cast<std::string>(xmlLength);
        
        
        //JOIN SP OK LF
        //Name:name LF
        //Version:version LF
        //Length:length LF
        //xml LF
        std::string response = "JOIN OK\nName:";
        response += spreadsheetName;
        response += "\nVersion:";
        response += boost::lexical_cast<std::string>(version);
        response += "\nLength:";
        response += length;
        response += "\n";
        response += xml;
        response += "\n";
        
        return response;
    }

    std::string create_Create_OK_Command(std::string spreadsheetName, std::string password) {
        
        //CREATE SP OK LF
        //Name:name LF
        //Password:password LF
        std::string response = "CREATE OK\nName:";
        response += spreadsheetName;
        response += "\nPassword:";
        response += password;
        response += "\n";
        
        
        return response;
    }

    std::string create_Create_FAIL_Command(std::string spreadsheetName, std::string message) {
        
        //CREATE SP FAIL LF
        //Name:name LF
        //message LF
        std::string response = "CREATE FAIL\nName:";
        response += spreadsheetName;
        response += "\n";
        response += message;
        response += "\n";
        

        return response;
    }

    std::string create_Change_OK_Command(std::string spreadsheetName, int version) {
        
        //CHANGE SP OK LF
        //Name:name LF
        //Version:version LF
        std::string response = "CHANGE OK\nName:";
        response += spreadsheetName;
        response += "\nVersion:";
        response += boost::lexical_cast<std::string>(version);
        response += "\n";
        
        return response;
    }

    std::string create_Change_FAIL_Command(std::string spreadsheetName, std::string message) {
        
        //CHANGE SP FAIL LF
        //Name:name LF
        //message LF
        std::string response = "CHANGE FAIL\nName:";
        response += spreadsheetName;
        response += "\n";
        response += message;
        response += "\n";
           
        return response;
    }

    std::string create_Change_WAIT_Command(std::string spreadsheetName, int version) {
        
        //CHANGE SP WAIT LF
        //Name:name LF
        //Version:version LF
        std::string response = "CHANGE WAIT\nName:";
        response += spreadsheetName;
        response += "\nVersion:";
        response += boost::lexical_cast<std::string>(version);
        response += "\n";
             
        return response;
    }

    std::string create_Undo_OK_Command(std::string spreadsheetName, int version, std::string cellName, std::string cellContents) {
        int contentsLength = cellContents.length();
        std::string length = boost::lexical_cast<std::string>(contentsLength);
        
        //UNDO SP OK LF
        //Name:name LF
        //Version:version LF
        //Cell:cell LF
        //Length:length LF
        //content LF
        std::string response = "UNDO OK\nName:";
        response += spreadsheetName;
        response += "\nVersion:";
        response += boost::lexical_cast<std::string>(version);
        response += "\nCell:";
        response += cellName;
        response += "\nLength:";
        response += length;
        response += "\n";
        response += cellContents;
        response += "\n";
        
        return response;
    }

    std::string create_Undo_END_Command(std::string spreadsheetName, int version) {
        
        //UNDO SP END LF
        //Name:name LF
        //Version:version LF
        std::string response = "UNDO END\nName:";
        response += spreadsheetName;
        response += "\nVersion:";
        response += boost::lexical_cast<std::string>(version);
        response += "\n";
        
        return response;
    }

    std::string create_Undo_WAIT_Command(std::string spreadsheetName, int version) {
        
        //UNDO SP WAIT LF
        //Name:name LF
        //Version:version LF
        std::string response = "UNDO WAIT\nName:";
        response += spreadsheetName;
        response += "\nVersion:";
        response += boost::lexical_cast<std::string>(version);
        response += "\n";

        return response;
    }

    std::string create_Undo_FAIL_Command(std::string spreadsheetName, std::string message) {
        
        //UNDO SP FAIL LF
        //Name:name LF
        //message LF
        std::string response = "UNDO FAIL\nName:";
        response += spreadsheetName;
        response += "\n";
        response += message;
        response += "\n";
        
        return response;
    }

    std::string create_Update_Command(std::string spreadsheetName, int version, std::string cellName, std::string cellContents) {
        int contentsLength = cellContents.length();
        std::string length = boost::lexical_cast<std::string>(contentsLength);
        
        //UPDATE LF
        //Name:name LF
        //Version:version LF
        //Cell:cell LF
        //Length:length LF
        //content LF
        std::string response = "UPDATE\nName:";
        response += spreadsheetName;
        response += "\nVersion:";
        response += boost::lexical_cast<std::string>(version);
        response += "\nCell:";
        response += cellName;
        response += "\nLength:";
        response += length;
        response += "\n";
        response += cellContents;
        response += "\n";

        return response;
    }

    std::string create_Save_OK_Command(std::string spreadsheetName) {
        
        //SAVE SP OK LF
        //Name:name LF
        std::string response = "SAVE OK\nName:";
        response += spreadsheetName;
        response += "\n";
        
        return response;
    }

    std::string create_Save_FAIL_Command(std::string spreadsheetName, std::string message) {
        
        //SAVE SP FAIL LF
        //Name:name LF
        //message LF
        std::string response = "SAVE FAIL\nName:";
        response += spreadsheetName;
        response += "\n";
        response += message;
        response += "\n";

        return response;
    }
}

