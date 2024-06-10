import { useSelectedChatContext } from "@/context/selectedChatContext";
import Image from "next/image";
import noChatPlaceholder from "../../public/img/no-chat.png";
import { IconDotsVertical, IconFileUpload, IconSend2 } from "@tabler/icons-react";
import { useEffect, useRef, useState } from "react";
import axios, { AxiosRequestConfig } from "axios";
import { Chat } from "@/models/chat";
import Messages from "./Messages";
import { Spinner } from "./Spinner";
import { Modal } from "./Modal";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { useChatContext, useChatContextProvider } from "@/context/chatContext";
import { decodeToken } from "@/utils/decodeToken";
import { ChatUser } from "@/models/chatUser";
import { useHubConnectionContext } from "@/context/hubConnectionContext";
import { Field, Form, Formik, FormikHelpers, FormikValues } from "formik";
import jwtClient from "jwt-client"; 
import { jwtDecode } from "jwt-decode";

const apiUrl = process.env.NEXT_PUBLIC_API_URL;
const authUrl = process.env.NEXT_PUBLIC_AUTH_URL;

type ChatProps = {
  title: string
};

type MessageFormValues = {
  message: string
};

export default function ChatRoom(){
    const {chatId} = useSelectedChatContext();
    const [chat, setChat] = useState<ChatUser>();
    const [chatInfo, setChatInfo] = useState<Chat>();
    const ChatProvider = useChatContextProvider();

    useEffect(() =>{
      axios.get<Chat>(`${apiUrl}/Chats/${chatId}`, {
        headers: {
          "Authorization" : localStorage.getItem(`Access-Token`)
        }, 
        withCredentials: true
      }).then((res) => {
        setChatInfo(res.data)
      })
    }, [chat]);

    useEffect(() =>{
      if (!chatId){
        return;
      }

      axios.get<ChatUser>(`${authUrl}/Tokens/chat-token/${chatId}`, {
        withCredentials: true,
        headers: {
          "Authorization": localStorage.getItem("Refresh-Token")
        }
      })
        .then((res) => {
         const chatToken: string = res.headers["authorization"];
          localStorage.setItem(`Chat-Token-${chatId}`, chatToken);

          setChat(res.data);
          
        }).catch((reason) => console.log(reason));

    }, [chatId]);

    if (!chatId){
        return <NoChat/>
    }

    if (!chatInfo){
      return <Spinner text="Загружаем данные чата..."/>
    }

    if (chatInfo){
      return (
        <ChatProvider value={chat!}>
          <Chat title={chatInfo.title}/>
        </ChatProvider>
      )
    }
}

function Chat({title}:ChatProps){
  const [active, setActive] = useState(false);
  const chatId = useSelectedChatContext();

  const initValues = {
    message: ""
  }

  const handleOnSubmit = async ({message}:MessageFormValues) => {
    await axios.post(`${apiUrl}/Chat/send-message`, {
      content: message
    }, {
      withCredentials: true,
      headers: {
        "Authorization": localStorage.getItem("Access-Token")
      }
    })
  }

  return(
      <div className="flex flex-col w-full h-full">
          <div className="flex flex-col p-4 justify-center items-center w-full h-full">
              <div className="flex items-center justify-center w-full border-slate-300 border-b border-solid pb-2">
                <div className="flex text-center bg-sky-700 py-2 px-4 text-white rounded-lg">
                    <p>{title}</p>
                    <button onClick={() => setActive(true)}><IconDotsVertical/></button>
                </div>
              </div>
              <Messages/>
              <div className="flex gap-1 justify-between max-h-min w-full items-center"> {/* Добавлено items-center */}
                <button className="flex items-center w-max h-full"><IconFileUpload className="h-10 w-10" /></button> {/* Добавлено items-center и h-full */}
                <Formik initialValues={initValues} 
                  onSubmit={handleOnSubmit}>
                  <Form className="flex w-full items-center gap-1">
                    <Field 
                      name="message"
                      as="textarea"
                      className="block border-[1px] border-slate-500 border-solid min-w-[70%] rounded-xl flex-grow"
                    /> 
                    <button className="h-full w-max flex items-center" type="submit"><IconSend2 className="h-10 w-10" /></button> 
                  </Form>
                </Formik>
              </div>
          </div>
          <Modal active={active} setActive={setActive}>
            <p>ggdggh</p>
          </Modal>
      </div>
  )
}

function NoChat(){
    return(
        <div className="flex flex-col justify-center items-center text-center h-full w-full">
          <Image src={noChatPlaceholder} alt="Выберите чат" width={100} objectFit="cover"/>
          <p>Выберите чат из списка или создайте новый, чтобы начать общение</p>
        </div>  
    );
}


