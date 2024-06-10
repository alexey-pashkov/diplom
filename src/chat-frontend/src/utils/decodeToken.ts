import { useJwt } from "react-jwt";

export function decodeToken<T>(token: string){
    const {decodedToken} = useJwt<T>(token);
    
    if (decodedToken == null){
        throw new Error("Unnable to decode token. Check, is token correct.")
    }
    else{
        return decodedToken
    }
}