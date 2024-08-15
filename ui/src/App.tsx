import React from "react"
import { MainLayout } from "./layouts/MainLayout/MainLayout"
import { MainPage } from "./pages/MainPage"
import { ProfileView } from "./components/Profile/Profiles"

export const metadata = {
  title: "IPTV Manager",
  description: "IPTV Manager",
}

const App = () => {

  const pages = {
    "main": <MainPage />,
    "profile": <ProfileView />
  }

  const [currentPage, setCurrentPage] = React.useState<keyof typeof pages>("profile")

  return (
    <MainLayout>
      {pages[currentPage]}
    </MainLayout>
  )
}

export default App
