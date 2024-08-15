import { HStack, IconButton, Text } from "@chakra-ui/react"
import { useMainLayout } from "../../layouts/MainLayout/MainLayout"
import { FaGear, FaMoon, FaSun } from "react-icons/fa6"
import { useProfile } from "../Providers/ProfileProvider"

export const NavBar = () => {
  const { toggleLeftMenuVisibility, leftMenuVisibility } = useMainLayout()
  const { setUseDarkTheme, useDarkTheme } = useProfile()
  return (
    <HStack
      width={"100%"}
      height={"40px"}
      alignItems={"center"}
      justifyContent={"end"}
      gap={4}
    >
      <IconButton
        aria-label={""}
        icon={<FaGear />}
        size={"sm"}
        onClick={toggleLeftMenuVisibility}
      />

      <IconButton
        aria-label={""}
        icon={useDarkTheme ? <FaSun /> : <FaMoon />}
        size={"sm"}
        onClick={() => setUseDarkTheme(!useDarkTheme)}
      />
    </HStack>
  )
}
