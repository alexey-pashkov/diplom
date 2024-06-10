'use client'

import Link from "next/link"
import { useEffect, useState } from "react"
import { fetcher } from "@/fetches";
import useSWR from "swr";
import { Chat } from "@/models/chat";
import { IconCheck, IconSearch, IconSquareRoundedPlus, IconUser, IconX } from "@tabler/icons-react";
import { AxiosRequestConfig } from "axios";
import Image from "next/image";
import chatPlaceholder  from "../../public/img/chat.png";
import { useSelectedChatContext } from "@/context/selectedChatContext";
import { Spinner } from "./Spinner";
import { Invitation } from "@/models/invitation";
import { Modal } from "./Modal";
import { UserCabinet } from "./UserCabinet";
import { NewChat } from "./NewChat";

const apiUrl = process.env.NEXT_PUBLIC_API_URL;

type Tabs = "chats" | "invitations";

type ChatListProps = { chats: Chat[] };
type AppNavigationHeaderProps = { setTab: (tab: Tabs) => void, tab:  Tabs};

export default function AppNavigation(){
    const [tab, setTab] = useState<Tabs>("chats");

    return(
        <div className="flex flex-col border-r-[1px] border-slate-300 border-solid min-w-[33%]">
            <AppNavigationHeader setTab={setTab} tab={tab}/>
            <div className="w-[100%]">
                {tab == "chats" ? <ChatList /> : <InvitationList/>}
            </div>
        </div>
    )
}

function AppNavigationHeader({setTab, tab}:AppNavigationHeaderProps){
    const [cabinetActive, setCabinetActive] = useState<boolean>(false);
    const [newChatActive, setNewChatActive] = useState<boolean>(false);

    const activeTabStyle = "bg-sky-700 rounded-t-lg p-2 w-[40%]";
    
    return(
        <div>
            <div className="flex flex-col pt-4 space-y-4 text-white bg-sky-600">
            <div className="flex justify-around">
                <button onClick={() => setCabinetActive(true)}><IconUser/></button>
                <button onClick={() => setNewChatActive(true)}><IconSquareRoundedPlus/></button>
                <button><IconSearch/></button>
            </div >
            <div className="flex justify-center mx-4">
                <button onClick={() => setTab("chats")} className={tab == "chats" ? activeTabStyle : " w-[40%]"}>Чаты</button>
                <button onClick={() => setTab("invitations")} className={tab == "invitations" ? activeTabStyle : " w-[40%]"}>Приглашения</button>
            </div>
        </div>
            <div className={cabinetActive ? "w-[100vw] h-[100vh]" : "hidden"}>
                <Modal active={cabinetActive} setActive={setCabinetActive}>
                    <UserCabinet />
                </Modal>
            </div>
            <div className={newChatActive ? "w-[100vw] h-[100vh]" : "hidden"}>
                <Modal active={newChatActive} setActive={setNewChatActive}>
                    <NewChat />
                </Modal>
            </div>
        </div>
        
        
    );

}

function InvitationList(){
    const {data, error, isLoading} = useSWR<Invitation[], any, {url: string, params: AxiosRequestConfig}>({
        url: `${apiUrl}/User/invitations`, 
        params: {
            method: "GET",
            headers:{
                "Authorization": `${localStorage.getItem("Access-Token")}`,
                "Content-Type": "application/json"
            },
            withCredentials: true
        }
    }, ({url, params}) => fetcher(url, params));

    if (isLoading){
        return <Spinner text="Загружаем приглашения..." />
    }
    
    return(
        <div className="bg-sky-700 text-white max-h-full overflow-y-scroll min-w-full">
            {
                data?.map((item) => {
                    return <InvitationEntry chatId={item.chatId} chatName={item.chatName} />
                })
            }
        </div>
    )
}

function InvitationEntry({chatId, chatName}:Invitation){
    return(
        <div className="flex flex-col border-b-[1px] space-x-4 border-solid border-slate-300/50 p-2 mx-2 h-min  hover:bg-sky-900 hover:rounded-md hover:mx-0">
            <p>Принять приглашениие в чат {chatName}?</p>
            <div className="flex justify-start gap-3">
                <button>
                    <IconCheck/>
                </button>
                <button>
                    <IconX/>
                </button>
            </div>
        </div>
    )
}

function ChatList(){
    const { data, error, isLoading } = useSWR<Chat[], any, {url: string, params: AxiosRequestConfig}>({
        url: `${apiUrl}/User/chats`, 
        params: {
            method: "GET",
            headers:{
                "Authorization": `${localStorage.getItem("Access-Token")}`,
                "Content-Type": "application/json"
            },
            withCredentials: true
        }
    }, ({url, params}) => fetcher(url, params));

    if (isLoading){
        return(
            <Spinner text="Загружаем чаты..."/>
        )
    }
    
    if (data){
        console.log(data);
    }

    if (error){
        <p>{error}</p>
    }

    return(
        <div className="bg-sky-700 text-white max-h-full overflow-y-scroll min-w-[100%]">
            {
                data?.map(chat => <ChatEntry 
                    chatId={chat.chatId}
                    title={chat.title}
                    created={chat.created} />)
            }
        </div>
    );
}

function ChatEntry({chatId, title, created}:Chat){
    const {setChatId} = useSelectedChatContext();
    
    const creationDate = new Date(created);
    
    return(
        <div className="flex border-b-[1px] space-x-4 border-solid border-slate-300/50 p-2 mx-2 h-min  hover:bg-sky-900 hover:rounded-md hover:mx-0"
        onClick={() => setChatId(chatId)}>
            <div className="rounded-[50%] max-w-[50px] max-h-min overflow-hidden">
                <Image src={chatPlaceholder} alt="w" objectFit="cover" />
            </div>
            <div className="flex flex-col">
                <h3>{title}</h3>
                <h2>Создан {creationDate.toLocaleString("ru-RU", {
                    year: "numeric",
                    month: "long",
                    day: "numeric"
                })}</h2>
            </div>
        </div>
    );
        
}