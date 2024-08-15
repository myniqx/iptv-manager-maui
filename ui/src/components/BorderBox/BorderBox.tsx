import { Box, BoxProps } from "@chakra-ui/react"
import { PropsWithChildren } from "react"

type BorderBoxProps = PropsWithChildren<BoxProps> & {
  when?: boolean
}

export const BorderBox: React.FC<BorderBoxProps> = ({
  children,
  when = true,
  ...props
}) => {
  if (!when) return null

  return (
    <Box
      {...props}
      border={"1px"}
      borderColor={"gray.300"}
      borderRadius={"lg"}
      p={"8px"}
      _focus={{ outline: "none", boxShadow: "outline" }}
    >
      {children}
    </Box>
  )
}
