 //spreadsheetManager.h

#ifndef SPREADSHEET_MANAGER_H
#define SPREADSHEET_MANAGER_H

#include "spreadsheet.h"
#include <vector>

class session;
class spreadsheet;

class spreadsheetManager{
    
public:
    
    
    void create_New_Spreadsheet_File(std::string spreadsheetName, std::string password, session* client);
    void join_Spreadsheet(std::string spreadsheetName, std::string password, session* client);
    void disconnect_Client_From_All(session* client);
    void disconnect_Client_From_Spreadsheet(std::string spreadsheetName, session* client);
    void disconnect_All_Clients_And_Save();
    void save_Spreadsheet(std::string spreadsheetName, session* clientName);
    void undo_Spreadsheet_Change(std::string spreadsheetName, int version, session* client);
    void change_Spreadsheet(std::string spreadsheetName, session* clientName, int version, std::string cellName, std::string cellContents);
    
    
private:
    
    std::map<std::string, spreadsheet*> spreadsheets;
    
    std::string create_Failed_Join_Command(std::string spreadsheetName, std::string message);
    
};


#endif
