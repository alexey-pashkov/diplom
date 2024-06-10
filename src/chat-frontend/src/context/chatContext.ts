import { ChatUser } from "@/models/chatUser";
import { createContext, useContext } from "react";

const context = createContext<ChatUser | null>(null);

export function useChatContextProvider(){
    return context.Provider;
}
export function useChatContext(){
    const data = useContext(context);

    if (!data){
        throw new Error(`"useContext" hook used outside of provider!`);
    }

    return data;
}