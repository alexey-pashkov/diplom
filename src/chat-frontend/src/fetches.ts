import axios, { AxiosRequestConfig } from "axios";
import { Chat } from "./models/chat";

const apiUrl = process.env.NEXT_PUBLIC_API_URL;

export async function fetcher(url: string, params: AxiosRequestConfig){
    const response = await axios({
        url: url,
        ...params
    });

    return response.data;
}
