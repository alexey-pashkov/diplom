type UserRole = "admin" | "moderator" | "user";

export type ChatUser = {
    userId: number,
    chatId: number,
    role: UserRole
}