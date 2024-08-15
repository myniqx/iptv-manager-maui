import {
  Card,
  CardHeader,
  Text,
  Flex,
  Avatar,
  Box,
  Heading,
  Image,
  IconButton,
  CardBody,
  CardFooter,
  Button,
} from "@chakra-ui/react"
import { FaHandDots } from "react-icons/fa6"

type EntityViewProps = {
  type?: string
  random?: boolean
}

export const EntityView: React.FC<EntityViewProps> = ({ random = false }) => {
  return (
    <Card variant="elevated" overflow={"hidden"}>
      <CardHeader>
        <Flex gap="4">
          <Flex flex="1" gap="4" alignItems="center" flexWrap="wrap">
            <Box>
              <Heading size="sm">Segun Adebayo</Heading>
              <Text>Creator, Chakra UI</Text>
            </Box>
          </Flex>
          <IconButton
            variant="ghost"
            colorScheme="gray"
            aria-label="See menu"
            icon={<FaHandDots />}
          />
        </Flex>
      </CardHeader>
      <CardBody>
        {false && (
          <Text>
            With Chakra UI, I wanted to sync the speed of development with the
            speed of design. I wanted the developer to be just as excited as the
            designer to create a screen.
          </Text>
        )}
      </CardBody>
      <Image
        aspectRatio={"1 / 3"}
        objectFit="cover"
        overflow={"hidden"}
        src={`https://picsum.photos/100/300`}
        alt="Chakra UI"
      />
    </Card>
  )
}
