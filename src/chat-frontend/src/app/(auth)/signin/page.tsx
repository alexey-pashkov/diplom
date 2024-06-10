'use client'

import axios from "axios";
import { ErrorMessage, Field, Form, Formik, FormikHelpers } from "formik";
import * as Yup from 'yup';
import { useRouter } from "next/navigation";
import { ReactNode } from "react";
import reactUseCookie from "react-use-cookie";

const authUrl = process.env.NEXT_PUBLIC_AUTH_URL;

type SignInvalues = {
    login: string,
    password: string,
    general?: string
}

const schema = Yup.object().shape({
    login: Yup.string()
      .required("Введите имя пользователя"),
    password: Yup.string()
      .required("Введите пароль")
    });

export default function SignUpPage(){
    const router = useRouter();
  
    const initialValues = {
      login: "",
      password: "",
      confirmPassword: ""
    };
  
    const handleOnSubmit = async ({ login, password }:SignInvalues,
      { setSubmitting, setErrors }: FormikHelpers<SignInvalues>
    ) => {
      try{
        const body = {
          login: login,
          password: password
        };
        axios.post<string>(`${authUrl}/Auth/signin`, {login, password})
          .then((res) => {
          localStorage.setItem("Access-Token", res.headers["authorization"]);
          
          localStorage.setItem("Refresh-Token", res.data)

          router.push("/main");
        }).catch((reason) => console.log(reason));     
      }
      catch(error){
        if (axios.isAxiosError(error)){
          if (error.response?.status == 401){
            setErrors({general: "Неверное имя пользователя или пароль"})
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
      <div className="flex flex-grow-0 flex-col-reverse md:flex-row justify-between h-full items-center md:rounded-2xl md:max-w-min md:min-h-max shadow-[0px_20px_20px_10px_#00000024] overflow-hidden">
          <div className="flex flex-grow-0 min-h-screen md:min-h-full justify-center bg-white">
            <div className="flex flex-col gap-4 items-center text-center m-6 min-h-full">
              <h3 className="text-3xl md:text-nowrap font-bold text-gray-700 mb-4" >Войдте в свой аккаунт</h3>
              <Formik initialValues={initialValues}
              onSubmit={handleOnSubmit}
              validationSchema={schema}
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
                  <ErrorMessage component={ErrorTemplate} name="general"/>
                  <button
                      type="submit"
                      disabled={isSubmitting || !isValid}
                      className="w-[250px] py-3 px-3 bg-blue-500 text-white rounded-lg border-2 border-white disabled:opacity-50 disabled:cursor-not-allowed disabled:pointer-events-none hover:bg-blue-600 focus:outline-none focus:ring-2 focus:ring-blue-400"
                    >
                      {isSubmitting == true ? "Выполняется вход..." : "Войти"}
                    </button>
                  <div>
                    {errors.general && <p className="text-center text-wrap">{errors.general}</p>}
                  </div>
                </Form>
                )}
              </Formik>
              <p>Еще не зарегистрированы? <a href="/signup" className="underline">Нажмите</a>, чтобы зарегистрироваться</p>
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