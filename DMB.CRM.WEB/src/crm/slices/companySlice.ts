import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import http from "../services/http.service";
import type { Company } from "../models";

export const fetchCompanies = createAsyncThunk("companies/list", async () => {
  const res = await http.get<Company[]>("/company/list");
  return res.data;
});

export const saveCompany = createAsyncThunk(
  "companies/save",
  async (payload: { id?: string; body: Partial<Company> }) => {
    const res = payload.id
      ? await http.put<Company>(`/company/${payload.id}`, payload.body)
      : await http.post<Company>("/company", payload.body);
    return res.data;
  }
);

const slice = createSlice({
  name: "companies",
  initialState: { items: [] as Company[], loading: false },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchCompanies.fulfilled, (state, action) => {
        state.items = action.payload;
        state.loading = false;
      })
      .addCase(saveCompany.fulfilled, (state, action) => {
        const idx = state.items.findIndex((c) => c.id === action.payload.id);
        if (idx >= 0) state.items[idx] = action.payload;
        else state.items.unshift(action.payload);
      });
  },
});

export default slice.reducer;
