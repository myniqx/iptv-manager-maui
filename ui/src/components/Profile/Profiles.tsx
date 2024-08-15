import { useState, useEffect } from "react"
import {
  Box,
  Flex,
  Input,
  Button,
  Text,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  InputGroup,
  InputLeftElement,
  Switch,
  FormControl,
  FormLabel,
} from "@chakra-ui/react"

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
import { AddProfile } from "./AddProfile"
import { Options } from "./Options"

// import { IptvManager } from './structures/iptvmanager'; // Adjust the path as needed
// import { Profile } from './structures/profiles/profile'; // Adjust the path as needed
/*
export class Profile {
    public groupCount: number = 0;
    public totalCount: number = 0;
    public tvShowSeasonCount: number = 0;
    public tvShowEpisodeCount: number = 0;
    public liveStreamCount: number = 0;
    public movieCount: number = 0;
    public tvShowCount: number = 0;

    public name: string;
    public createdDate: number;
    public updatedDate: number;
    public url: string;
    public id: number;
}
*/

type Profile = {
  groupCount: number
  totalCount: number
  tvShowSeasonCount: number
  tvShowEpisodeCount: number
  liveStreamCount: number
  movieCount: number
  tvShowCount: number
  name: string
  createdDate: number
  updatedDate: number
  url: string
  id: number
  lastUpdated: number
}
export const ProfileView = () => {
  // const iptv = IptvManager.getInstance();
  const [mUrl, setMUrl] = useState("")
  const [profiles, setProfiles] = useState([] as Profile[])
  const [sProf, setSProf] = useState<Profile | undefined>(undefined)
  const [deleteProf, setDeleteProf] = useState(-1)
  const [status, setStatus] = useState("ready...")
  const [loadOn, setLoadOn] = useState(true)
  const [switchTheme, setSwitchTheme] = useState(
    document.documentElement.classList.contains("dark"),
  )

  useEffect(() => {
    selectFirst()
  }, [])

  const add = () => {
    // iptv.addProfile(mUrl);
    setMUrl("")
    setDeleteProf(-1)
  }

  const selProf = (p: Profile) => {
    setSProf(p)
    setDeleteProf(-1)
  }

  const fillTable = (prof: Profile) => {
    const created = new Date(prof.createdDate)
    const formattedDate = `${(created.getDate() + 1)
      .toString()
      .padStart(2, "0")}-${(created.getMonth() + 1)
        .toString()
        .padStart(
          2,
          "0",
        )}-${created.getFullYear()} ${created.getHours()}:${created.getMinutes()}`
    return [
      [<LinkIcon />, "Url", prof.url],
      [<InfoOutlineIcon />, "Created Date", formattedDate],
      [<UploadIcon />, "Last Update", prof.lastUpdated],
      [<FolderIcon />, "Group", prof.groupCount],
      [<FilmIcon />, "Movies", prof.movieCount],
      [<LiveTvIcon />, "Live Streams", prof.liveStreamCount],
      [<TvIcon />, "TvShows", prof.tvShowCount],
      [<FolderIcon />, " > Seasons", prof.tvShowSeasonCount],
      [<FolderIcon />, " > Episodes", prof.tvShowEpisodeCount],
      [<InfoOutlineIcon />, "Total", prof.totalCount],
    ]
  }

  const selectFirst = () => {
    /*
    for (const p of iptv.profileMap.values()) {
      selProf(p);
      return;
    }
    setDeleteProf(-1);
    */
  }

  const load = async (p: Profile) => await doCatalog(p, false)
  const update = async (p: Profile) => await doCatalog(p, true)

  const doCatalog = async (p: Profile, update: boolean) => {
    // emits("onLoad", null); // You'll need to handle this emit in React
    // const c = await iptv.loadCatalog(p, update, (msg: string) => setStatus(msg));
    // onLoad(c); // You'll need to handle this emit in React
  }

  const remove = () => {
    //  iptv.remProfile(deleteProf);
    selectFirst()
  }

  const handleThemeChange = () => {
    setSwitchTheme(!switchTheme)
    //  iptv.setTheme(!switchTheme);
  }

  return (
    <Flex direction="column" w="full" h="screen">

      <AddProfile />

      <Box w="full">
        {profiles.length === 0 ? (
          <Text fontSize="4xl" fontWeight="medium" textAlign="center" py={10}>
            Enter your iptv url up and create your first profile.
          </Text>
        ) : (
          <>
            <Box
              w="full"
              bg={deleteProf >= 0 ? "red.400" : "transparent"}
              border="1px"
              borderRadius="md"
              borderColor="gray.200"
              p={3}
              _dark={{
                bg: deleteProf >= 0 ? "red.400" : "transparent",
                borderColor: "gray.600",
              }}
            >
              {deleteProf >= 0 ? (
                <Flex
                  direction="column"
                  alignItems="center"
                  gap={2}
                  fontSize="xl"
                  fontWeight="medium"
                >
                  <Text>'profile#{deleteProf}' ll be deleted?</Text>
                  <Flex gap={4}>
                    <Button colorScheme="blue" size="sm" onClick={remove}>
                      Okey
                    </Button>
                    <Button
                      colorScheme="blue"
                      size="sm"
                      onClick={() => setDeleteProf(-1)}
                    >
                      Cancel
                    </Button>
                  </Flex>
                </Flex>
              ) : (
                <Text fontFamily="mono" fontSize="sm">
                  Status : {status}
                </Text>
              )}
            </Box>

            <Flex direction={{ base: "column", lg: "row" }} mt={2}>
              <Box
                width={{ base: "full", lg: "25%" }}
                fontSize="sm"
                fontWeight="medium"
                overflowY="auto"
                display="flex"
                flexWrap="wrap"
                flexDirection={{ base: "row", lg: "column" }}
                border="1px"
                borderRadius="md"
                borderColor="gray.200"
                bg="white"
                _dark={{ bg: "gray.800", borderColor: "gray.600" }}
              >
                {profiles.map((p, id) => (
                  <Button
                    key={id}
                    onClick={() => selProf(p)}
                    colorScheme={sProf === p ? "blue" : "gray"}
                    variant={sProf === p ? "solid" : "ghost"}
                    m={1}
                    px={4}
                    py={2}
                    border="1px"
                    borderRadius="md"
                    borderColor="gray.200"
                    _dark={{ borderColor: "gray.600" }}
                  >
                    {p.name}
                  </Button>
                ))}
              </Box>

              {sProf && (
                <Flex
                  direction={{ base: "column", lg: "row" }}
                  w="full"
                  overflowX="hidden"
                  border="1px"
                  borderRadius="md"
                  borderColor="gray.200"
                  bg="white"
                  _dark={{ bg: "gray.800", borderColor: "gray.600" }}
                >
                  <Table
                    size="sm"
                    variant="simple"
                    w="full"
                    borderRight="1px"
                    borderRightColor="gray.600"
                  >
                    <Tbody>
                      {fillTable(sProf).map((el, index) => (
                        <Tr
                          key={index}
                          _even={{ bg: "gray.100", _dark: { bg: "gray.700" } }}
                        >
                          <Th
                            scope="row"
                            px={6}
                            py={1}
                            w="1/3"
                            display="flex"
                            alignItems="baseline"
                          >
                            {el[0]}
                            <Text ml={1}>{el[1]}</Text>
                          </Th>
                          <Td
                            px={6}
                            py={1}
                            w="2/3"
                            textAlign="center"
                            textOverflow="ellipsis"
                            overflow="hidden"
                          >
                            {el[2]}
                          </Td>
                        </Tr>
                      ))}
                    </Tbody>
                  </Table>

                  <Flex
                    direction={{ base: "row-reverse", lg: "column" }}
                    width={{ base: "full", lg: "20%" }}
                    p={2}
                    gap={2}
                  >
                    <Button
                      colorScheme="blue"
                      size="sm"
                      onClick={() => load(sProf)}
                    >
                      Load
                    </Button>
                    <Button
                      colorScheme="blue"
                      size="sm"
                      onClick={() => update(sProf)}
                    >
                      Update{" "}
                      <Text as="span" fontSize="xs" color="black">
                        {sProf.lastUpdated}
                      </Text>
                    </Button>
                    <Button
                      colorScheme="red"
                      size="sm"
                      onClick={() => setDeleteProf(sProf.id)}
                    >
                      Delete
                    </Button>
                  </Flex>
                </Flex>
              )}
            </Flex>
          </>
        )}
      </Box>

      <Options />
    </Flex>
  )
}
