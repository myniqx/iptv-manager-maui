import { Heading, HStack, VStack } from "@chakra-ui/react"
import { EntityView } from "../EntityView/EntityView"

type HorizontalGroupProps = {
  title?: string
}

export const HorizontalGroup: React.FC<HorizontalGroupProps> = ({
  title = "Untitled yet",
}) => {
  return (
    <VStack width="100%" overflowX={"auto"} alignItems={"start"} my={2}>
      <Heading size="md" textAlign={"left"} ml={4}>
        {title}
      </Heading>
      <HStack my={2} gap={3}>
        {Array.from({ length: 4 }).map((_, index) => (
          <EntityView key={index} random={true} />
        ))}
      </HStack>
    </VStack>
  )
}
