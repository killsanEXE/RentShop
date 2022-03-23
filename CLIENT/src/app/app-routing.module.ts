import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProfileComponent } from './account/profile/profile.component';
import { RegAndLogComponent } from './account/reg-and-log/reg-and-log.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { AdminGuard } from './guards/admin.guard';
import { AuthGuard } from './guards/auth.guard';
import { HomeComponent } from './home/home.component';
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
      {path: "item/:id", component: ItemDetailedComponent, resolve: {item: ItemDetailedResolver}}
    ]
  },
  {path: "regandlog", component: RegAndLogComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
