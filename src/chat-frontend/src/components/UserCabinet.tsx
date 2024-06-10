import { IconUser } from "@tabler/icons-react";
import { Field, Form, Formik, FormikHelpers, FormikValues } from "formik";
import { useEffect, useState } from "react";
import jwtClient from "jwt-client"
import axios from "axios";
import { UserInfo } from "@/models/userInfo";

const apiUrl = process.env.NEXT_PUBLIC_API_URL;

type UserCabinetProps = {
    userId: number,
    login: string
};

type Values = {};

type LoginFormValues = {
    login: string
};

type ChangePasswordFormValues = {
    newPassword: string,
    confirmNewPassword: string
}

export function UserCabinet(){
    const [login, setLogin] = useState<LoginFormValues>();
    const [id, setId] = useState<number>();
    const [loginChanged, setLoginChanged] = useState<boolean>(false);

    useEffect(() => {
        const accessToken = localStorage.getItem("Access-Token")!;
        const {claim} = jwtClient.read(accessToken);
        
        setId(parseInt(claim.nameid, 10));
        setLogin({login: claim.unique_name});
    }, []);

    const handleChangeLogin = async ({login}:LoginFormValues) => {
        const newInfo = await axios.patch<Omit<UserInfo, "chats">>(`${apiUrl}/Users/${id}`, {
            login: login
        }, {
            withCredentials: true,
            headers: {
                "Authorization": localStorage.getItem("Access-Token")
            }
        });

        if (newInfo.status == 200){
            setLogin({login: newInfo.data.login});
            setLoginChanged(true);
        }

        
    }

    return(
        <div className="flex flex-col p-4">
            <div className="flex">
                <IconUser />
                <h1 className="font-bold text-2xl">Личный кабинет</h1>
            </div>
            <Formik initialValues={login!} 
                enableReinitialize={true}
                onSubmit={handleChangeLogin}>
                <Form>
                    <p>Имя пользователя</p>
                    <p className={loginChanged ? "text-green-700" : "hidden"}>Логин успешно изменен!</p>
                    <Field 
                        className="border border-slate-300 rounded-lg p-2"
                        type="text"
                        name="login"
                    />
                    <button className="w-fit h-min py-3 px-3 bg-blue-500 text-white rounded-lg
                         border-2 border-white disabled:opacity-50 disabled:cursor-not-allowed 
                         disabled:pointer-events-none hover:bg-blue-600 focus:outline-none 
                         focus:ring-2 focus:ring-blue-400">
                            Сохранить
                    </button>
                </Form>
            </Formik>
            <Formik initialValues={{}} onSubmit={function (values: FormikValues, formikHelpers: FormikHelpers<FormikValues>): void | Promise<any> {
                throw new Error("Function not implemented.");
            } }>
                <Form>
                    <p>Смена пароля</p>
                    <div className="flex flex-col p-2 border border-slate-300 rounded-xl">
                        <p>Новый пароль</p>
                        <Field 
                            className="border border-slate-300 rounded-lg p-2"
                            type="password"
                        />
                        <p>Подтверждение пароля</p>
                        <Field 
                            className="border border-slate-300 rounded-lg p-2"
                            type="password"
                        />
                        <div className="flex flex-row-reverse">
                        <button className="w-fit h-min py-3 px-3 bg-blue-500 text-white rounded-lg
                            border-2 border-white disabled:opacity-50 disabled:cursor-not-allowed 
                            disabled:pointer-events-none hover:bg-blue-600 focus:outline-none 
                            focus:ring-2 focus:ring-blue-400">
                                Изменить
                        </button>
                    </div>
                    </div>
                </Form>
            </Formik>
        </div>
    )
}