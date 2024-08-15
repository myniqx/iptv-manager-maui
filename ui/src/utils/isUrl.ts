

export const isURL = (str: string): boolean => {
  const urlRegex = /^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([/\w.-]*)*/;
  return urlRegex.test(str);
};
