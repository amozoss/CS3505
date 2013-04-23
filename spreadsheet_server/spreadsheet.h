//spreadsheet.h


#ifndef SPREADSHEET_H
#define SPREADSHEET_H


#include <map>
#include <string>
#include <stack>
#include "server.hpp"

class session;

class spreadsheet{

public:
    spreadsheet();
    spreadsheet(std::string filename);
    ~spreadsheet();

    void change_Cell(session* client, int version, std::string cellName, std::string cellContents);
    void load_Spreadsheet_From_File(std::string filename);
    bool add_Client(session* client, std::string password);
    bool disconnect_Client(session* client);
    void disconnect_All_Clients();
    void client_Requested_Save(session* client);
    void undo_Last_Change(session* clientName, int version);
    bool isInValidState();

private:
    std::map<std::string, std::string> cells;
    std::map<session*, session*> clients;
    std::stack<std::string>* undoCellNameStack;
    std::stack<std::string>* undoCellContentsStack;
    std::string password;
    std::string name;
    int version;
    bool isValidState;
    
    //Helper Methods
    void save();
    bool save_To_File();
    bool load_From_File(std::string filename);
    std::string generateXML();
        


};

namespace IO_Helper {
    bool create_Spreadsheet(std::string name, std::string password);
    bool fileExists(const std::string& filename); 
}


#endif