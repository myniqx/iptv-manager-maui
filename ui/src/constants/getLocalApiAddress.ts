

const localApiAddress = "http://localhost:5000/"

export type Endpoints = "settings" | "profile"

export const getLocalApiAddress = (endpoint: Endpoints, ...args: string[]) => localApiAddress + endpoint + "/" + args.join("/")
