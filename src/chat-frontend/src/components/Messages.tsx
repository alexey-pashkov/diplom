import { useSelectedChatContext } from "@/context/selectedChatContext";
import { fetcher } from "@/fetches";
import { Message } from "@/models/message";
import { AxiosRequestConfig } from "axios";
import { useEffect } from "react";
import { useJwt } from "react-jwt";
import useSWR from "swr";
import { Spinner } from "./Spinner";

const apiUrl = process.env.NEXT_PUBLIC_API_URL;

type MessageProps = {userId: number} & Omit<Message, "id">;

export default function Messages(){
  const {chatId} = useSelectedChatContext();

  const {decodedToken} = useJwt<{userId: number}>(localStorage.getItem(`Chat-Token-${chatId}`)!);

  const { data, error, isLoading } = useSWR<Message[], any, {url: string, params: AxiosRequestConfig}>({
    url: `${apiUrl}/Chat/messages`, 
    params: {
        method: "GET",
        headers: {
          "Authorization" : localStorage.getItem(`Chat-Token-${chatId}`)
        },
        withCredentials: true
    }
  }, ({url, params}) => fetcher(url, params), {
    refreshInterval: 500
  });


  if (isLoading){
    return <Spinner text="Загружаем сообщения..."/>
  }

  if (data){
    return(
      <div className="flex flex-col-reverse justify-self-end py-2 h-full w-full self-end overflow-x-scroll custom-scrollbar">
        {
          data?.map((val) => <MessageEntry user={decodedToken?.userId!} userId={val.user} content={val.content}/>)
        }
      </div>
    );
  }
}

function MessageEntry({user, userId, content}:MessageProps){
  const theme = user == userId ? "self-end bg-slate-500" : "self-start bg-sky-600";

  return(
    <div className={"max-w-[35%] p-2 my-2 rounded-xl " + theme}>
      {content}
    </div>
  );
}