import { IconX } from "@tabler/icons-react";
import { ReactNode } from "react";


type ModalProps = {children: ReactNode, active: boolean, setActive: (val: boolean) => void};

export function Modal({active, setActive, children}:ModalProps){
    return(
        <div className={active ? 
            "bg-black fixed top-0 left-0 w-full h-full flex items-center justify-center bg-opacity-50 transition-opacity duration-300 z-50" 
            : "hidden"}>
            <div className="self-center justify-self-center absolute flex flex-col rounded-lg bg-white">
                <div className="flex-row w-full px-2 pt-2">
                    <button onClick={() => (setActive(false))}><IconX/></button>
                </div>
                {children}
            </div>
        </div>
    )
}