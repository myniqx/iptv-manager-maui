import { Flex, FormLabel, InputGroup, InputLeftElement, Input, Button, VStack } from "@chakra-ui/react"
import React from "react"
import {
  RiLinksLine as LinkIcon,
  RiInformationLine as InfoOutlineIcon,
  RiUploadCloud2Line as UploadIcon,
  RiFolderLine as FolderIcon,
  RiMovie2Line as FilmIcon,
  RiLiveLine as LiveTvIcon,
  RiTv2Line as TvIcon,
  RiRefreshLine as RefreshIcon,
  RiPlayLine as PlayIcon,
  RiDeleteBinLine as DeleteIcon,
} from "react-icons/ri"
import { useProfile } from "../Providers/ProfileProvider"
import { isURL } from "../../utils/isUrl"


export const AddProfile = () => {
  const [mUrl, setMUrl] = React.useState<string>("")
  const { addProfileUrl } = useProfile()

  const add = () => {
    addProfileUrl(mUrl)
    setMUrl("")
  }

  return (
    <VStack
      w="full"
      alignItems="start"
      justifyContent="space-between"
      p={2}
      mb={2}
      border="1px"
      borderRadius="md"
      borderColor="gray.200"
      bg="white"
      _dark={{ bg: "gray.800", borderColor: "gray.600" }}
    >
      <FormLabel htmlFor="mUrl" mb={2} fontSize="sm" fontWeight="medium">
        Add Profile
      </FormLabel>
      <InputGroup>
        <InputLeftElement pointerEvents="none">
          <LinkIcon color="gray.300" />
        </InputLeftElement>
        <Input
          id="mUrl"
          type="url"
          placeholder="enter your iptv link here"
          value={mUrl}
          onChange={(e) => setMUrl(e.target.value)}
          pl={10}
          pr={20}
          size="sm"
        />
        <Button
          onClick={add}
          isDisabled={!isURL(mUrl)}
          colorScheme="blue"
          size="sm"
          ml={2}>
          Add
        </Button>
      </InputGroup>
    </VStack>
  )
}
