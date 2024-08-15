import React from "react";
import { Endpoints, getLocalApiAddress } from "../constants/getLocalApiAddress";

export const useSSE = <T extends object>(endpoint: Endpoints, ...args: string[]) => {
  const [data, setData] = React.useState<T>({} as T);

  React.useEffect(() => {
    const source = new EventSource(getLocalApiAddress(endpoint, ...args));

    source.onmessage = (event) => {
      try {
        const parsedData = JSON.parse(event.data);
        setData(parsedData);
      } catch (error) {
        console.error("Error parsing SSE data:", error);
      }
    };

    return () => source.close();
  }, [endpoint, ...args]);

  return data;
};
