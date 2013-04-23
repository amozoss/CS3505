//spreadsheet.cc

#include "spreadsheet.h"
#include <sys/stat.h>
#include <fstream>
#include <sstream>
#include "ClientCommands.hpp"
#include <boost/lexical_cast.hpp>

//constructors and de-constructors 

spreadsheet::spreadsheet() {
    undoCellNameStack = new std::stack<std::string>;
    undoCellContentsStack = new std::stack<std::string>;
    isValidState = false;
}

spreadsheet::spreadsheet(std::string name) {
    undoCellNameStack = new std::stack<std::string>;
    undoCellContentsStack = new std::stack<std::string>;
    load_Spreadsheet_From_File(name);
    
    
}

spreadsheet::~spreadsheet() {
    delete undoCellNameStack;
    delete undoCellContentsStack;
}

bool spreadsheet::isInValidState(){
    return isValidState;
}





void spreadsheet::change_Cell(session* client, int version, std::string cellName, std::string cellContents) {

    
    std::map<session*,session*>::iterator clientIT = clients.find(client);
    if ( clientIT != clients.end()) {
        if (version == this->version) {

            //save old content into the save stacks
            undoCellNameStack->push(cellName);
            undoCellContentsStack->push(cells[cellName]);

            cells[cellName] = cellContents;
            this->version++;
            
            client->send_msg(ClientCommands::create_Change_OK_Command(name, this->version));
            
            //Update all other clients
            std::map<session*,session*>::iterator otherClientIT = clients.begin();
            for(; otherClientIT != clients.end(); otherClientIT++){
                if(otherClientIT == clientIT){
                    continue;
                }
                //otherClientIT->second.send_msg(ClientCommands::create_Update_Command(name, this->version, cellName, cellContents) );
                
                session* client2 = otherClientIT->second;
                client2->send_msg(ClientCommands::create_Update_Command(name, this->version, cellName, cellContents));
            }
            return;
            
            
            //return NULL;
        } else {
            client->send_msg(ClientCommands::create_Change_WAIT_Command(name, this->version));
            return;
        }
    } else {
        //return "Client is not logged into this spreadsheet editing session.";
        client->send_msg(ClientCommands::create_Change_FAIL_Command(name, "Client is not currently logged into this spreadsheet editing session"));
        return;
    }

}//Completed

void spreadsheet::save() {
    if (isValidState) {
        //save the spreadsheet
        //clear the undo stack
        delete undoCellNameStack;
        delete undoCellContentsStack;
        undoCellNameStack = new std::stack<std::string>;
        undoCellContentsStack = new std::stack<std::string>;
        save_To_File();
    }
}

void spreadsheet::load_Spreadsheet_From_File(std::string sheetName) {

    this->name = sheetName;
    version = 0;
    sheetName.append(".ss");
    isValidState = this->load_From_File(sheetName);
    
}

bool spreadsheet::add_Client(session* client, std::string password) {
    if(isValidState) {
        if (password.compare( this->password) == 0 ) {
            clients[client] = client;
            client->send_msg(ClientCommands::create_Join_OK_Command(name, version, this->generateXML()));
            return true;
        }
    }
    

    return false;
}

std::string spreadsheet::generateXML(){
    std::string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?><spreadsheet version=\"";
    xml += boost::lexical_cast<std::string>(this->version);
    xml += "\">";
    
    std::map<std::string, std::string>::iterator it = cells.begin();
    
    for(; it != cells.end(); it++){
      if (it->first.compare("") == 0)
	continue;
        xml+= "<cell><name>";
        xml += it->first;
        xml += "</name><contents>";
        xml += it->second;
        xml += "</contents></cell>";
    }
    xml+="</spreadsheet>";
    
    return xml;
}


/**
 * Return true if all clients have been disconnected.
 * Otherwise False if at least one client is connected.
 */

bool spreadsheet::disconnect_Client(session* client) {
    //remove client from list
    clients.erase(client);
    //if there are no more clients
    if (clients.size() == 0) {
        this->save();
        return true;
    }
    
    return false;
}

//

void spreadsheet::disconnect_All_Clients() {

    this->save();
    isValidState = false;
    

}

void spreadsheet::client_Requested_Save(session* client) {
    if (isValidState) {
        if (clients.find(client) != clients.end()) {
            this->save();
            client->send_msg(ClientCommands::create_Save_OK_Command(name));
        } else {
            std::string message = "Can not save. Client is not logged into this spreadsheet editing session.";
            client->send_msg(ClientCommands::create_Save_FAIL_Command(name, message  ));
        }
    }
    //return "Spreadsheet doesn't exist.";
}

void spreadsheet::undo_Last_Change(session* client, int version) {
    std::map<session*,session*>::iterator clientIT = clients.find(client);
    if ( clientIT != clients.end()) {
        if (version == this->version) {
            if (undoCellNameStack->empty()) {
                client->send_msg(ClientCommands::create_Undo_END_Command(name, this->version));
            }
            std::string cellName = undoCellNameStack->top();
            std::string cellContents = undoCellContentsStack->top();

            undoCellNameStack->pop();
            undoCellContentsStack->pop();
            cells[cellName] = cellContents;
            this->version++;
            //confirm with client
            client->send_msg(ClientCommands::create_Undo_OK_Command(name, this->version, cellName, cellContents));
            //update all other clients
            std::map<session*,session*>::iterator otherClientIT = clients.begin();
            for(; otherClientIT != clients.end(); otherClientIT++){
                if(otherClientIT == clientIT){
                    continue;
                }
                session* client2 = otherClientIT->second;
                //session* client2 = *otherClientIT;
                client2->send_msg(ClientCommands::create_Update_Command(name, this->version, cellName, cellContents));
            }
            return;
            
        } else {
            //return "Client's spreadsheet is out of date.  Please wait for updated spreadsheet.";
            client->send_msg(ClientCommands::create_Undo_WAIT_Command(name, this->version));
            return;
        }
    } else {
        std::string message = "Client is not logged into this spreadsheet editing session.";
        client->send_msg(ClientCommands::create_Undo_FAIL_Command(name, message));
        return;
    }
}

bool spreadsheet::save_To_File() {
    std::string filename = name;
    filename += ".ss";
    std::ofstream saveFile(filename.c_str());
    std::string line = password;
    if(saveFile.is_open()){
        line.append("\n");
        saveFile << password << std::endl;
        std::map<std::string,std::string>::iterator it = cells.begin();
        for(;it != cells.end(); it++){
            std::string line = it->first;
            line.append(":");
            line.append(it->second);
            
            saveFile << line << std::endl;
        }
        saveFile.close();
        
        return true;
    }
    return false;
    
}

bool spreadsheet::load_From_File(std::string filename) {
    std::string line;
    std::ifstream file;
    file.open(filename.c_str());
    if (file.is_open()) {
        getline(file, line);
        this->password = line;
        while (!file.eof()) {

            std::getline(file, line);
            std::string cellName;
            std::string cellContents;
            std::stringstream cellLine(line);
        
            
            std::getline(cellLine, cellName, ':');
            std::getline(cellLine, cellContents);
            cells[cellName] = cellContents;
           
        }
        file.close();
        return true;
    }
    return false;
    
}



namespace IO_Helper {

    bool create_Spreadsheet(std::string name, std::string password) {

        name.append(".ss");
        if (fileExists(name)) {
            return false;
        }
        std::ofstream saveFile(name.c_str());
        std::string line = password;
        if (saveFile.is_open()) {
            line.append("\n");
            saveFile << password;
            saveFile.close();
            return true;
        }
        
        return false;
    }

    // Function: fileExists
    //From user Rico on http://stackoverflow.com/questions/4316442/stdofstream-check-if-file-exists-before-writing
    //With mod by Matt

    /**
        Check if a file exists
    @param[in] filename - the name of the file to check

    @return    true if the file exists, else false

     */
    bool fileExists(const std::string& filename) {
        struct stat buf;
        return (stat(filename.c_str(), &buf) != -1);

    }

}
