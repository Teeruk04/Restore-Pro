import { ButtonGroup, Button } from "@mui/material";
import { useAppDispatch, useAppSelector } from "../../App/store/configureStore";
import { decremented, incremented } from "./counterSlice";

const ContactPage = () => {
  const dispatch = useAppDispatch()
  const {num} = useAppSelector((state)=>state.counter)

  return (
    <>
      <ButtonGroup
        variant="contained"
        aria-label="outlined primary button group"
      >
        <Button onClick={()=>dispatch(decremented(5))}>-</Button>
        <Button>
          {num}
        </Button>
        <Button onClick={()=>dispatch(incremented(5))}>+</Button>
      </ButtonGroup>
    </>
  );
};
export default ContactPage;


