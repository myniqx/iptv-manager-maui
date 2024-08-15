
import React, { SetStateAction } from "react"
import { PropsWithChildren } from "react"
import { usePreference } from "../../hooks/usePreference"
import { getLocalApiAddress } from "../../constants/getLocalApiAddress"
import { useToast } from "@chakra-ui/react"

type ProfileContextProps = {
  useDarkTheme: boolean
  setUseDarkTheme: (value: SetStateAction<boolean>) => void
  loadProfileOnStart: boolean
  setLoadProfileOnStart: (value: SetStateAction<boolean>) => void

  getAllProfiles: () => void
  profiles: never[]
  addProfileUrl: (url: string) => void
}

const ProfileContext = React.createContext<ProfileContextProps>({} as ProfileContextProps)

export const ProfileProvider: React.FC<PropsWithChildren> = ({
  children
}) => {
  const [useDarkTheme, setUseDarkTheme] = usePreference<boolean>("darkTheme", false)
  const [loadProfileOnStart, setLoadProfileOnStart] = usePreference<boolean>("loadProfileOnStart", true)
  const [profiles, setProfiles] = React.useState([])
  const toast = useToast()

  const getAllProfiles = () => {
    const fetchAsync = async () => {
      try {
        const result = await fetch(getLocalApiAddress("profile", "getAll"), {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        })

        if (!result.ok)
          throw new Error(`Error: ${result.status} ${result.statusText}`);

        const profiles = await result.json()
        setProfiles(profiles)
      } catch (error) {
        toast({
          title: "Profiles couldn't be fetched",
          description: "Profiles couldn't be fetched",
          status: "error",
          duration: 3000,
          isClosable: true,
        })
      }
    }

    fetchAsync()
  }

  const addProfileUrl = (url: string) => {
    const fetchAsync = async () => {
      try {
        const result = await fetch(getLocalApiAddress("profile", "addUrl"), {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ url }),
        })

        if (!result.ok)
          throw new Error(`Error: ${result.status} ${result.statusText}`);

        getAllProfiles()
      }
      catch (error) {
        toast({
          title: "M3U url couldn't be added",
          description: `${url} couldn't be added`,
          status: "error",
          duration: 3000,
          isClosable: true,
        })
      }
    }

    fetchAsync()
  }

  return (
    <ProfileContext.Provider
      value={{
        useDarkTheme,
        setUseDarkTheme,
        loadProfileOnStart,
        setLoadProfileOnStart,
        getAllProfiles,
        profiles,
        addProfileUrl
      }}
    >
      {children}
    </ProfileContext.Provider>
  )
}

export const useProfile = () => React.useContext(ProfileContext) 
