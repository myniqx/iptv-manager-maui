import { Box, Heading, Grid, Text } from "@chakra-ui/react"
import { HorizontalGroup } from "../components/HorizontalGroup/HorizontalGroup"



export const MainPage = () => {
  return (
    <Box p={4}>
      <Heading as="h1" size="lg" mb={4}>
        Dashboard
      </Heading>
      <Grid templateColumns="repeat(3, 1fr)" gap={6}>
        <Box bg="gray.100" p={4} borderRadius="md">
          <Heading as="h2" size="md" mb={2}>
            Widget 1
          </Heading>
          <Text>Widget 1 content</Text>
        </Box>
        <Box bg="gray.100" p={4} borderRadius="md">
          <Heading as="h2" size="md" mb={2}>
            Widget 2
          </Heading>
          <Text>Widget 2 content</Text>
        </Box>
        <Box bg="gray.100" p={4} borderRadius="md">
          <Heading as="h2" size="md" mb={2}>
            Widget 3
          </Heading>
          <Text>Widget 3 content</Text>
        </Box>
      </Grid>
      <HorizontalGroup />
    </Box>
  )
}
