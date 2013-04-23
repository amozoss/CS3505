#include "spreadsheetManager.h"
#include "spreadsheet.h"
#include <cctype>
#include "ClientCommands.hpp"

//spreadsheetManager.cc

bool isValidName(std::string name){
    
    for(int i =0; i < name.length()-1; i++){
        if(!isalnum(name[i])){
            return false;
        }
    }
    return true;
}

void spreadsheetManager::create_New_Spreadsheet_File(std::string spreadsheetName, std::string password, session* client)
{
    
    //check for valid name - valid name is a string that contains only alphanumeric characters.
    
    // if name is not valid
    if(!isValidName(spreadsheetName)){
        //return "Invalid spreadsheet name: Name may only contain alphanumeric characters.";
        std::string message = "Invalid spreadsheet name:  Name may only contain alphanumeric character.";
        std::string response = ClientCommands::create_Create_FAIL_Command(spreadsheetName, message);
        client->send_msg(response);
        return;
    }   
    
    if( IO_Helper::create_Spreadsheet(spreadsheetName, password) ){
        client->send_msg(ClientCommands::create_Create_OK_Command(spreadsheetName, password) );
    } else {
        client->send_msg(ClientCommands::create_Create_FAIL_Command(spreadsheetName, "Spreadsheet already exists.") );
    }
    
}

/**
 * Associates a client with a specific spreadsheet if the password supplied by the client matches the spreadsheet
 * 
 * Sends a response to the client notifying of the success or failure.
 * 
 * @param spreadsheetName
 * @param password
 * @param client
 * @return 
 */
void spreadsheetManager::join_Spreadsheet(std::string spreadsheetName, std::string password, session* client)
{
    //check for valid spreadsheet name - valid name is a valid filename without the path and extension.
    
    // if name is not valid
    if(!isValidName(spreadsheetName)){
        std::string message = "Invalid name: the spreadsheet name can only contain alphanumeric characters.";
        std::string response = ClientCommands::create_Join_FAIL_Command(spreadsheetName, message);
        client->send_msg(response);
        
        return;
    }
    
    //check if there is a spreadsheet session by the spreadsheet name
    //if spreadsheet is active
    std::map<std::string,spreadsheet*>::iterator it = spreadsheets.find(spreadsheetName);
    if( it != spreadsheets.end() ){
        //attempt to add client to the session
        spreadsheet* sheet = it->second;
        sheet->add_Client(client, password);
        return;
        
        
    }
    // (no active sheet session)
    
    //append extension .ss to name
    std::string filename = "./" + spreadsheetName;
    filename += ".ss";
    
    //check for the existence of the file based on name
    //if file doesn't exist
    if( !IO_Helper::fileExists(filename) ){
        //set error to "Spreadsheet does not exist."
        std::string message = "Spreadsheet or password does not match.";
        std::string response = ClientCommands::create_Join_FAIL_Command(spreadsheetName, message);
        client->send_msg(response);
                
        return;
    }
        
        //create spreadsheet from file
    spreadsheet* sheet = new spreadsheet(spreadsheetName);
        //attempt to add client to the session

    if(!sheet->add_Client(client, password)){
        std::string message = "Spreadsheet or password does not match.";
        std::string response = ClientCommands::create_Join_FAIL_Command(spreadsheetName, message);
        client->send_msg(response);
        return;
    } else{
        spreadsheets[spreadsheetName] = sheet;
        
        
    }
        //if vector == NULL   (client wasn't added due to invalid password)
                //delete created spreadsheet
                //set error to "Invalid password"
                //return vector which is NULL
        //else (client was added)
                //add created spreadsheet to the active session list
                //set error to NULL
                //return vector which should be a valid vector
    

}

/**
 * Disconnects a client from a spreadsheet if it exists.
 * @param spreadsheetName
 * @param clientName
 */
void spreadsheetManager::disconnect_Client_From_Spreadsheet(std::string spreadsheetName, session* client){
    
    //if spreadsheet exists
    if(spreadsheets.find(spreadsheetName) != spreadsheets.end()){
        //remove client from spreadsheet
        if(spreadsheets[spreadsheetName]->disconnect_Client(client)){
            spreadsheets.erase(spreadsheetName);
        }
    }
} //completed

void spreadsheetManager::disconnect_Client_From_All(session* client){
  for(std::map<std::string,spreadsheet*>::iterator it = spreadsheets.begin(); it != spreadsheets.end(); it++){
    //disconnects client from any spreadsheet it is attached to.
    it->second->disconnect_Client(client);
    //delete the session
    //delete client;
  }
}

void spreadsheetManager::disconnect_All_Clients_And_Save(){
    
    //foreach active session
    
    for(std::map<std::string,spreadsheet*>::iterator it = spreadsheets.begin(); it != spreadsheets.end(); it++){
        //disconnects all clients and automatically saves when the last client has been removed.
        it->second->disconnect_All_Clients();
        //delete spreadsheet
        //spreadsheets.erase(it);
        
    }
}

void spreadsheetManager::save_Spreadsheet(std::string spreadsheetName, session* client)
{
    //if spreadsheet exists
    std::map<std::string,spreadsheet*>::iterator it = spreadsheets.find(spreadsheetName);
    if( it != spreadsheets.end() ){
        it->second->client_Requested_Save(client);
    }
}
void spreadsheetManager::undo_Spreadsheet_Change(std::string spreadsheetName, int version, session* client)
{
    std::map<std::string,spreadsheet*>::iterator it = spreadsheets.find(spreadsheetName);
    if( it != spreadsheets.end() ){
        it->second->undo_Last_Change(client, version);
    }
}
void spreadsheetManager::change_Spreadsheet(std::string spreadsheetName, session* client, int version, std::string cellName, std::string cellContents)
{
    std::map<std::string,spreadsheet*>::iterator it = spreadsheets.find(spreadsheetName);
    if( it != spreadsheets.end() ){
        it->second->change_Cell(client, version, cellName, cellContents);
    }
}


    
