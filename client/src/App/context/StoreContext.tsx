import { createContext, PropsWithChildren, useContext, useState } from "react";
import { Basket } from "../model/Basket";

interface StoreContextValue {
  basket: Basket | null;
  setBasket: (basket: Basket) => void;
  removeItem: (productId: number, quantity: number) => void;
}
//ก ําหนดค่ําไว้ก่อน(สร้ํางห้องครัวเปล่ําๆ)
export const StoreContext = createContext<StoreContextValue | undefined>(
  undefined
);
//2.ถูกเรียกใช้จํากภํายนอก
export function useStoreContext() {
  const context = useContext(StoreContext);
  if (context === undefined) {
    throw Error("Oops - we do not seem to be inside the provider");
  }
  return context;
}
//1.สร้ํางสเตทไว้ภํายใน และน ําไปครอบ { children } ที่ต้องกํารใช้(Index.tsx)
export function StoreProvider({ children }: PropsWithChildren<any>) {
  const [basket, setBasket] = useState<Basket | null>(null);
  
  function removeItem(productId: number, quantity: number) {
    if (!basket) return;
    const items = [...basket.items];
    const itemIndex = items.findIndex((i) => i.productId === productId);
    if (itemIndex >= 0) {
      items[itemIndex].quantity -= quantity;
      if (items[itemIndex].quantity === 0) items.splice(itemIndex, 1);
      setBasket((prevState) => {
        return { ...prevState!, items };
      });
    }
  }
  return (
    //value แมบจับคู่กับ interface StoreContextValue
    // {children} ที่ถูกครอบจะสํามํารถเข้ําถึงค่ําของ value ได้
    <StoreContext.Provider value={{ basket, setBasket, removeItem }}>
      {children}
    </StoreContext.Provider>
  );
}
