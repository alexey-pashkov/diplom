import Image from "next/image";
import spinner from "../../public/img/spinner.svg";

type SpinnerProps = {text?: string};

export function Spinner({text}:SpinnerProps){
    return(
        <div className="flex flex-col gap-2 justify-center items-center text-center h-full w-full">
            <Image 
                src={spinner}
                width={100}
                alt="Загрузка" 
            />
            <p>{text ?? ""}</p>
        </div>
    )
}