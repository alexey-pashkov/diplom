'use client'

import { UserInfo } from "@/models/userInfo";
import axios from "axios";
import { ErrorMessage, Field, Form, Formik, FormikHelpers } from "formik";
import { useRouter } from "next/navigation";
import { ReactNode } from "react";
import * as Yup from 'yup';

const authUrl = process.env.NEXT_PUBLIC_AUTH_URL;

type SignUpValues= {
  login: string, 
  password: string,
  confirmPassword: string,
  general?: string
};

const schema = Yup.object().shape({
  login: Yup.string()
    .min(8, "Имя пользователя должно содержать не менее 8 символов")
    .max(32, "Имя пользователя должно содержать не более 32 символов")
    .required("Введите имя пользователя"),
  password: Yup.string()
    .min(8, "Пароль должен содержать не менее 8 символов")
    .max(64, "Пароль должен содержать не более 64 символов")
    .required("Введите пароль"),
  confirmPassword: Yup.string()
    .oneOf([Yup.ref('password')], "Пароли должны совпадать")
    .required("Подтвердите свой пароль")
});

export default function SignUpPage(){
  const router = useRouter();

  const initialValues = {
    login: "",
    password: "",
    confirmPassword: ""
  };

  const handleOnSubmit = async ({ login, password }:SignUpValues,
    { setSubmitting, setErrors }: FormikHelpers<SignUpValues>
  ) => {
    try{
      const {headers} = await axios.post<UserInfo>(`${authUrl}/Auth/signup`, {login, password})

      localStorage.setItem("Access-Token", headers["authorization"]);
        
      router.push("/main");
    }
    catch(error){
      if (axios.isAxiosError(error)){
        if (error.response?.status == 409){
          setErrors({login: "Пользователь с таким логином уже существует"})
        }
        if(error.response?.status == 500){
          setErrors({general: "Произошла неизвестная ошибка. Попробуйте еще раз"})
        }
      }
    }
    finally{
      setSubmitting(false);
    }
  }

  return (
    <div className="flex flex-grow-0 flex-col-reverse md:flex-row justify-between items-center md:rounded-2xl md:max-w-min md:min-h-max shadow-[0px_20px_20px_10px_#00000024] overflow-hidden">
        <div className="flex flex-col items-center justify-center text-center self-stretch m-4">
          <h3 className="text-2xl font-bold md:text-nowrap text-gray-700 mb-4 block">Уже зарегистрированы?</h3>
          <p className="block mb-4">Войдите в свой аккаунт, чтобы продолжить общение</p>
          <a href="/signin" className="w-full p-3 bg-blue-500 text-white rounded-lg border-2 border-white max-w-32 hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-400">
            Войти
          </a>
        </div>
        <div className="flex flex-grow-0 justify-center bg-sky-500">
          <div className="flex flex-col items-center text-center m-4 min-h-full">
            <h3 className="text-3xl md:text-nowrap font-bold text-gray-700 mb-4" >Присоединяйтесь к нам!</h3>
            <Formik initialValues={initialValues}
            validationSchema={schema}
            onSubmit={handleOnSubmit}
            >
              {({isSubmitting, errors, isValid}) => (
                <Form className="flex flex-col items-center min-h-max">
                <div className="mb-4">
                  <Field
                    type="text"
                    name="login"
                    placeholder="Имя пользователя"
                    className="p-3 border md:w-[250px] border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400"
                  />
                  <ErrorMessage component={ErrorTemplate} name="login" />
                </div>
                <div className="mb-4">
                  <Field
                    type="password"
                    name="password"
                    placeholder="Пароль"
                    className="md:w-[250px] p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400"
                  />
                  <ErrorMessage component={ErrorTemplate} name="password" />
                </div>
                <div className="mb-6">
                  <Field
                    type="password"
                    name="confirmPassword"
                    placeholder="Повторите пароль"
                    className="md:w-[250px] p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400"
                  />
                  <div>
                    <ErrorMessage name="confirmPassword" />
                  </div>
                </div>
                <button
                    type="submit"
                    disabled={isSubmitting || !isValid}
                    className="w-fit py-3 px-3 bg-blue-500 text-white rounded-lg border-2 border-white disabled:opacity-50 disabled:cursor-not-allowed disabled:pointer-events-none hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-400"
                  >
                    {isSubmitting == true ? "Регистрируем вас..." : "Зарегистрироваться"}
                  </button>
                <div>
                  {errors.general && <p className="text-center text-wrap">{errors.general}</p>}
                </div>
              </Form>
              )}
            </Formik>
          </div>
        </div>
    </div>
  );
};

function ErrorTemplate({children}:{children?: ReactNode}){
  return(
    <div className="text-center break-words max-w-full text-sm">
      <p>{children}</p>
    </div>
  )
}