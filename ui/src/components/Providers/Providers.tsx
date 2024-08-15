
import React, { PropsWithChildren } from "react"
import { ProfileProvider } from "./ProfileProvider"
import { ChakraProvider } from "./ChakraProvider"

export const Providers: React.FC<PropsWithChildren> = ({ children }) => {


  return (
    <ProfileProvider>
      <ChakraProvider>
        {children}
      </ChakraProvider>
    </ProfileProvider>
  )
}
