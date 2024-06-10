'use client'

import AppNavigation from "@/components/AppNavigation";
import ChatRoom from "@/components/ChatRoom";
import { useHubConnectionContextProvider } from "@/context/hubConnectionContext";
import { useSelectedChatContextProvider } from "@/context/selectedChatContext";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { useState } from "react";

const hubUrl = process.env.NEXT_PUBLIC_HUB_URL ?? "";

export default function MainPage(){
    const [chatId, setChatId] = useState<number | null>(null);

    const HubConnectionProvider = useHubConnectionContextProvider();
    const SelectedChatProvider = useSelectedChatContextProvider();

    return(
        <SelectedChatProvider value={{chatId, setChatId}}>
            <HubConnectionProvider value={
                new HubConnectionBuilder()
                    .withUrl(hubUrl, {accessTokenFactory() {
                        return localStorage.getItem("Access-Token")!
                    }})
                    .build()
            }>
                <main className="flex h-[100vh]">
                    <AppNavigation/>
                    <ChatRoom />
                </main>
            </HubConnectionProvider>
        </SelectedChatProvider>
    )
}