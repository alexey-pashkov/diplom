import { Chat } from "@/models/chat";
import axios from "axios";
import { Field, Form, Formik, FormikHelpers, FormikValues } from "formik";
import { useState } from "react";

const apiUrl = process.env.NEXT_PUBLIC_API_URL;

type NewChatValues = {
    chatName: string
}

export function NewChat(){
    const initVals = {
        chatName: ""
    };

    const [chatAdded, setChatAdded] = useState<boolean>(false);
    
    const handleonSubmit = ({chatName}:NewChatValues) =>{
        axios.post<Chat>(`${apiUrl}/Chats`, {
            chatName: chatName
        }, {
            withCredentials: true,
            headers: {
                "Authorization": localStorage.getItem("Access-Token")
            }
        }).then((res) =>{
            alert(res);
            if (res.status == 200){
                setChatAdded(true);

                setTimeout(() => setChatAdded(false), 3000);
            }
        }).catch((reason) => alert(reason));
    };

    return(
        <div className="p-2">
            <h1 className="font-bold text-2xl">Создать новый чат</h1>
            <p className={chatAdded ? "text-green-700" : "hidden"}>Чат успешно добавален!</p>
            <Formik initialValues={initVals} onSubmit={handleonSubmit}>
                <Form className="flex flex-col gap-3">
                    <p>Введите имя чата:</p>
                    <Field
                        className="border border-slate-300 rounded-lg p-2"
                        type="text"
                        name="chatName"
                        placeholder="Имя нового чата"
                    />
                    <div className="flex flex-row-reverse w-full">
                        <button className="w-fit h-min py-3 px-3 bg-blue-500 text-white rounded-lg
                         border-2 border-white disabled:opacity-50 disabled:cursor-not-allowed 
                         disabled:pointer-events-none hover:bg-blue-600 focus:outline-none 
                         focus:ring-2 focus:ring-blue-400">
                            Создать
                        </button>
                    </div>
                </Form>
            </Formik>
        </div>
    )
}