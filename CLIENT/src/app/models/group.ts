export interface Group{
    name: string; 
    connections: Connection[];
    unreadMessages: boolean;
    lastMessageSender: string;
    lastMessageContent: string;
    username1: string;
    username2: string;
    user1Photo: string;
    user2Photo: string;
}

interface Connection{
    connectionId: string;
    username: string;
}