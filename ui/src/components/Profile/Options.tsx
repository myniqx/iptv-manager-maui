import { Flex, FormControl, FormLabel, Switch } from "@chakra-ui/react"
import { useProfile } from "../Providers/ProfileProvider"



export const Options = () => {

  const { useDarkTheme, setUseDarkTheme, loadProfileOnStart, setLoadProfileOnStart } = useProfile()

  return (
    <Flex
      p={6}
      gap={2}
      mt={2}
      border="1px"
      borderRadius="md"
      borderColor="gray.200"
      _dark={{ borderColor: "gray.600" }}
    >
      <FormControl display="flex" alignItems="center" flexGrow={1}>
        <FormLabel htmlFor="loadOn" mb="0" mr={2}>
          On start, automatically load last profile
        </FormLabel>
        <Switch
          id="loadOn"
          isChecked={loadProfileOnStart}
          onChange={(e) => setLoadProfileOnStart(e.target.checked)}
        />
      </FormControl>
      <FormControl display="flex" alignItems="center" flexGrow={1}>
        <FormLabel htmlFor="switchTheme" mb="0" mr={2}>
          Dark Theme
        </FormLabel>
        <Switch
          id="switchTheme"
          isChecked={useDarkTheme}
          onChange={(e) => setUseDarkTheme(e.target.checked)}
        />
      </FormControl>
    </Flex>
  )
}
