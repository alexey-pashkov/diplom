import { HubConnection } from "@microsoft/signalr";
import { createContext, useContext } from "react";

const context = createContext<HubConnection | null>(null);

export function useHubConnectionContextProvider(){
    return context.Provider;
}
export function useHubConnectionContext(){
    const data = useContext(context);

    if (!data){
        throw new Error(`"useContext" hook used outside of provider!`);
    }

    return data;
}