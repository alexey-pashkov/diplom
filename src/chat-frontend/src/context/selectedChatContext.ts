import { Dispatch, SetStateAction, createContext, useContext } from "react";

type ChatContext = {chatId: number | null, setChatId: Dispatch<SetStateAction<number | null>>};

const ctx = createContext<ChatContext | null>(null);

export function useSelectedChatContextProvider(){
    return ctx.Provider;
}
export function useSelectedChatContext(){
    const data = useContext(ctx);

    if (!data){
        throw new Error(`"useContext" hook used outside of provider!`);
    }

    return data;
}