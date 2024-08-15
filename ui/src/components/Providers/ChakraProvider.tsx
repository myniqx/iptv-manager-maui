
import { ChakraProvider as ChakraThemeProvider, ColorModeScript, extendTheme, useColorMode } from "@chakra-ui/react"
import { PropsWithChildren, useEffect, useMemo } from "react"
import { useProfile } from "./ProfileProvider"


export const ChakraProvider: React.FC<PropsWithChildren> = ({ children }) => {
  const { useDarkTheme } = useProfile()
  const { setColorMode } = useColorMode()

  const theme = useMemo(() => {
    const config = {
      initialColorMode: useDarkTheme ? 'dark' : 'light',
      useSystemColorMode: false,
    }
    console.log("useDarkTheme", useDarkTheme, config)
    return extendTheme({ config })
  }, [useDarkTheme])


  console.log("useDarkTheme", useDarkTheme, theme)

  return (
    <>
      <ColorModeScript initialColorMode={theme.config.initialColorMode} />
      <ChakraThemeProvider theme={theme}>
        {children}
      </ChakraThemeProvider>
    </>
  )
}
