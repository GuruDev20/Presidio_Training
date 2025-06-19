import { createFeatureSelector,createSelector } from "@ngrx/store";
import { User } from "../models/user.model";

export const selectUsersState=createFeatureSelector<User[]>('users');

export const selectUsers=createSelector(selectUsersState,state => state);