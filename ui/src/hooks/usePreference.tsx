import { SetStateAction, useEffect, useState } from "react";
import { getLocalApiAddress } from "../constants/getLocalApiAddress";

type PreferenceValue = string | number | boolean | object | null;

export const usePreference = <T extends PreferenceValue>(key: string, defaultValue: T): [T, (value: SetStateAction<T>) => void] => {
  const [storedValue, setStoredValue] = useState<T>(defaultValue);

  useEffect(() => {
    const fetchValue = async () => {
      try {
        const body = JSON.stringify({ Key: key, DefValue: defaultValue });
        const result = await fetch(getLocalApiAddress("settings", "get"), {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body,
        });
        console.log(body)
        if (!result.ok) {
          throw new Error("Failed to fetch preference");
        }

        const value = await result.json() as T;
        setStoredValue(value as T);
      } catch (error) {
        console.error("Error fetching preference:", error);
      }
    };

    fetchValue();
  }, [key]);

  const setValueLocal = async (value: SetStateAction<T>) => {
    const newValue = typeof value === "function" ? (value as (prevState: T) => T)(storedValue) : value;
    try {
      const body = JSON.stringify({ Key: key, Value: newValue });
      const result = await fetch(getLocalApiAddress("settings", "set"), {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body,
      });
      console.log(body)

      if (!result.ok) {
        throw new Error("Failed to set preference");
      }

      setStoredValue(newValue);
    } catch (error) {
      console.error("Error setting preference:", error);
      if (process.env.NODE_ENV === "development") {
        console.log(key, " >> ", newValue);
        setStoredValue(newValue);
      }
    }
  };

  return [storedValue, setValueLocal];
};
