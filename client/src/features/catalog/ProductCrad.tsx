import {
  Card,
  CardMedia,
  CardContent,
  Typography,
  CardActions,
  Button,
  CardHeader,
  Avatar,
  IconButton,
} from "@mui/material";
import { red } from "@mui/material/colors";
import React, { useState } from "react";
import { Product } from "../../App/model/Product";
import MoreVertIcon from "@mui/icons-material/MoreVert";
import { Link } from "react-router-dom";
import { LoadingButton } from "@mui/lab";
import agent from "../../App/api/agent";
import { useAppDispatch, useAppSelector } from "../../App/store/configureStore";
import { addBasketItemAsync, setBasket } from "../basket/basketSlice";
interface Props {
  product: Product;
}

const ProductCrad = ({ product }: Props) => {
  const { status } = useAppSelector((state) => state.basket);
  const dispatch = useAppDispatch();
  
  return (
    <>
      <Card sx={{ maxWidth: "100%" }}>
        <CardHeader
          avatar={
            <Avatar sx={{ bgcolor: red[500] }} aria-label="recipe">
              {product.name.at(0)?.toUpperCase()}
            </Avatar>
          }
          action={
            <IconButton aria-label="settings">
              <MoreVertIcon />
            </IconButton>
          }
          title={product.name}
          subheader={product.brand}
        />
        <CardMedia
          component="img"
          alt="green iguana"
          height="140"
          sx={{
            backgroundColor: "#DEF5F4",
          }}
          image={product.pictureUrl}
        />
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {(product.price / 100).toFixed(2)} ฿
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {product.brand} / {product.type}
          </Typography>
        </CardContent>
        <CardActions>
          <LoadingButton
            loading={status ==="pendingAddItem" + product.id}
            onClick={() =>dispatch(addBasketItemAsync({ productId: product.id }))}
            size="small"
          >
            Add To Cart
          </LoadingButton>
          <Button component={Link} to={`/catalog/${product.id}`} size="small">
            View
          </Button>
        </CardActions>
      </Card>
    </>
  );
};

export default ProductCrad;
