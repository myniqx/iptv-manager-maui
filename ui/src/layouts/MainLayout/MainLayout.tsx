import {
  Box,
  HStack,
  useBreakpoint,
  useBreakpointValue,
} from "@chakra-ui/react"
import { LeftMenu } from "../../components/LeftMenu/LeftMenu"
import { PropsWithChildren } from "react"
import { BorderBox } from "../../components/BorderBox/BorderBox"
import { NavBar } from "../../components/NavBar/NavBar"
import React from "react"

type MainLayoutContextProps = {
  leftMenuVisibility: boolean
  setLeftMenuVisibility: (visibility: boolean) => void
  toggleLeftMenuVisibility: () => void
}

const MainLayoutContext = React.createContext<MainLayoutContextProps>(
  {} as MainLayoutContextProps,
)

export const MainLayout: React.FC<PropsWithChildren> = ({ children }) => {
  const [leftMenuVisibility, setLeftMenuVisibility] = React.useState(true)
  const toggleLeftMenuVisibility = () =>
    setLeftMenuVisibility(!leftMenuVisibility)

  return (
    <HStack width={"100vw"} height={"100vh"}>
      <MainLayoutContext.Provider
        value={{
          leftMenuVisibility,
          setLeftMenuVisibility,
          toggleLeftMenuVisibility,
        }}
      >
        <BorderBox
          when={leftMenuVisibility}
          width={"250px"}
          flexShrink={0}
          display={{ base: "none", lg: "block" }}
          height={"95vh"}
          my={"2.5vh"}
          ml={"12px"}
          bg={"gray.200"}
        >
          <LeftMenu />
        </BorderBox>
        <Box flexGrow={1} height={"100vh"} py={"6px"} pr={"8px"} bg={"gray.50"}>
          <NavBar />
          {children}
        </Box>
      </MainLayoutContext.Provider>
    </HStack>
  )
}

export const useMainLayout = () => React.useContext(MainLayoutContext)
