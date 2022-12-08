import { Typography, Grid, Paper, Box, Button } from "@mui/material";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import AppTextInput from "../../App/components/AppTextInput";
import { Product } from "../../App/model/Product";
interface Props {
  product?: Product; //กรณีเป็น ? หมายถึง สามารถก าหนดค่าเป็น undefined
  cancelEdit: () => void;
}
export default function ProductForm({ product, cancelEdit }: Props) {
  const { control, reset } = useForm();
  useEffect(() => {
    if (product) reset(product); //ถ้ามีสินค้าให้น ามาใส่ในฟอร์มด้วยค าสั่ง reset
  }, [product, reset]);
  return (
    <Box component={Paper} sx={{ p: 4 }}>
      <Typography variant="h4" gutterBottom sx={{ mb: 4 }}>
        Product Details
      </Typography>
      <Grid container spacing={3}>
        <Grid item xs={12} sm={12}>
          <AppTextInput control={control} name="name" label="Product name" />
        </Grid>
        <Grid item xs={12} sm={6}>
          <AppTextInput control={control} name="brand" label="Brand" />
        </Grid>
        <Grid item xs={12} sm={6}>
          <AppTextInput control={control} name="type" label="Type" />
        </Grid>
        <Grid item xs={12} sm={6}>
          <AppTextInput control={control} name="price" label="Price" />
        </Grid>
        <Grid item xs={12} sm={6}>
          <AppTextInput
            control={control}
            name="quantityInStock"
            label="Quantity in Stock"
          />
        </Grid>
        <Grid item xs={12}>
          <AppTextInput
            control={control}
            name="description"
            label="Description"
          />
        </Grid>
        <Grid item xs={12}>
          <AppTextInput control={control} name="pictureUrl" label="Image" />
        </Grid>
      </Grid>
      <Box display="flex" justifyContent="space-between" sx={{ mt: 3 }}>
        <Button variant="contained" color="inherit" onClick={cancelEdit}>
          Cancel
        </Button>
        <Button variant="contained" color="success">
          Submit
        </Button>
      </Box>
    </Box>
  );
}
