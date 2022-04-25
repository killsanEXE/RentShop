import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ForgotPasswordComponent } from './account/forgot-password/forgot-password.component';
import { ProfileComponent } from './account/profile/profile.component';
import { RegAndLogComponent } from './account/reg-and-log/reg-and-log.component';
import { ResendEmailConfirmationComponent } from './account/resend-email-confirmation/resend-email-confirmation.component';
import { ResetPasswordComponent } from './account/reset-password/reset-password.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { EditItemComponent } from './admin/items/edit-item/edit-item.component';
import { PointComponent } from './admin/points/point/point.component';
import { DeliverymanMainComponent } from './deliveryman/deliveryman-main/deliveryman-main.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { AdminGuard } from './guards/admin.guard';
import { AuthGuard } from './guards/auth.guard';
import { DeliverymanGuard } from './guards/deliveryman.guard';
import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages.component';
import { ClientOrdersComponent } from './orders/client-orders/client-orders.component';
import { ItemDetailedComponent } from './orders/item-detailed/item-detailed.component';
import { ItemDetailedResolver } from './resolvers/item-detailed.resolver';

const routes: Routes = [
  {path: "", component: HomeComponent},
  {
    path: "",
    runGuardsAndResolvers: "always",
    canActivate: [AuthGuard],
    children: [
      {path: "profile", component: ProfileComponent},
      {path: "admin", component: AdminPanelComponent, canActivate: [AdminGuard]},
      {path: "admin/edit-item/:id", component: EditItemComponent, canActivate: [AdminGuard], resolve: {item: ItemDetailedResolver}},
      {path: "admin/points", component: PointComponent, canActivate: [AdminGuard]},
      {path: "item/:id", component: ItemDetailedComponent, resolve: {item: ItemDetailedResolver}},
      {path: "orders", component: ClientOrdersComponent},
      {path: "deliveries", component: DeliverymanMainComponent, canActivate: [DeliverymanGuard]},
      {path: "messages", component: MessagesComponent}
    ]
  },
  {path: "regandlog", component: RegAndLogComponent},
  {path: "forgot-password", component: ForgotPasswordComponent},
  {path: "reset-password", component: ResetPasswordComponent},
  {path: "resend-email-confirmation", component: ResendEmailConfirmationComponent},
  {path: "server-error", component: ServerErrorComponent},
  {path: "**", component: NotFoundComponent, pathMatch: "full"}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
